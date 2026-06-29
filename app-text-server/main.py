from io import BytesIO
from fastapi import FastAPI, HTTPException, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from pypdf import PdfReader
from docx import Document


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

    print("I RECEIVED FILENAME:")
    print(filename)
    print("CONTENT IS TYPE:")
    print(type(content))

    if filename.endswith('.pdf'):
        text = extract_text_pdf(content)
    elif filename.endswith('.docx'):
        text = extract_text_docx(content)
    elif filename.endswith('.txt'):
        text = extract_text_txt(content)
    else:
        raise HTTPException(status_code=400, detail='Unsupported file format')
    
    return {
        "fileName" : filename,
        "text" : text
    }
    
def extract_text_pdf(content : bytes) -> str:
    reader = PdfReader(BytesIO(content))
    pages = []

    for page in reader.pages:

        text = page.extract_text() or ""

        pages.append(text)

    return "\n".join(pages).strip()

def extract_text_docx(content : bytes) -> str:
    document = Document(BytesIO(content))
    paragraphs = [p.text for p in document.paragraphs if p.text.strip()]

    return "\n".join(paragraphs).strip()

def extract_text_txt(content : bytes) -> str:
    try:
        return content.decode("utf-8").strip()
    except UnicodeDecodeError:
        return content.decode("latin-1").strip()
    