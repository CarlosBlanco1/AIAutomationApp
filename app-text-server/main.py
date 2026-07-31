from contextlib import asynccontextmanager
from fastapi import FastAPI, HTTPException, Request, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from app.text_pre_processing import EmbeddedChunk, TextPreProcessing, TextChunk
from transformers import AutoTokenizer
from sentence_transformers import SentenceTransformer

class PromptEmbeddingRequest(BaseModel):
    prompt : str

@asynccontextmanager
async def lifespan(app: FastAPI):
    app.state.embedding_model = SentenceTransformer("BAAI/bge-large-en-v1.5")
    app.state.tokenizer = AutoTokenizer.from_pretrained("BAAI/bge-large-en-v1.5")

    yield

    del app.state.embedding_model
    del app.state.tokenizer
    
app = FastAPI(lifespan=lifespan)

app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost:8080",
        "http://myapp-api:8080"
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.get("/")
def say_hello():
    return "Hello There!"

@app.post("/generate-embedded-chunks")
async def extract_text(request : Request, file : UploadFile):
    filename = file.filename.lower()
    content = await file.read()

    text = ''

    tokenizer = request.app.state.tokenizer
    embedding_model = request.app.state.embedding_model

    if filename.endswith('.pdf'):
        text = TextPreProcessing.extract_text_pdf(content)
    elif filename.endswith('.docx'):
        text = TextPreProcessing.extract_text_docx(content)
    elif filename.endswith('.txt'):
        text = TextPreProcessing.extract_text_txt(content)
    else:
        raise HTTPException(status_code=400, detail='Unsupported file format')
    
    text = TextPreProcessing.normalize_text(text)
    text_chunks : list[TextChunk] = TextPreProcessing.chunk_text(text, tokenizer)
    embedded_text_chunks : list[EmbeddedChunk] = TextPreProcessing.embed_chunks(text_chunks, embedding_model)
    
    return embedded_text_chunks

@app.post("/generate-prompt-embedding")
def generate_prompt_embedding(request : Request, body : PromptEmbeddingRequest) -> list[float]:
    embedding_model = request.app.state.embedding_model

    return embedding_model.encode(body.prompt, normalize_embeddings=True).tolist()
