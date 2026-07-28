from io import BytesIO
from fastapi import FastAPI, HTTPException, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from text_pre_processing import TextPreProcessing
from transformers import AutoTokenizer
from sentence_transformers import SentenceTransformer

app = FastAPI()

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

@app.post("/text-extractor")
async def extract_text(file : UploadFile):
    filename = file.filename.lower()
    content = await file.read()

    tokenizer = AutoTokenizer.from_pretrained("BAAI/bge-large-en-v1.5")
    model = SentenceTransformer("BAAI/bge-large-en-v1.5")



    if filename.endswith('.pdf'):
        text = TextPreProcessing.extract_text_pdf(content)
    elif filename.endswith('.docx'):
        text = TextPreProcessing.extract_text_docx(content)
    elif filename.endswith('.txt'):
        text = TextPreProcessing.extract_text_txt(content)
    else:
        raise HTTPException(status_code=400, detail='Unsupported file format')
    
    return {
        "fileName" : filename,
        "text" : text
    }
