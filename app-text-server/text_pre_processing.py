from dataclasses import dataclass
from io import BytesIO
import unicodedata
from pypdf import PdfReader
from docx import Document

@dataclass
class EmbeddedChunk:
    index : int
    chunk : str
    vector : list[float]
    token_size : int

@dataclass
class TextChunk:
    text : str
    token_size : int

class TextPreProcessing:
    @staticmethod
    def extract_text_pdf(content : bytes) -> str:
        reader = PdfReader(BytesIO(content))
        pages = []

        for page in reader.pages:

            text = page.extract_text() or ""

            pages.append(text)

        return "\n".join(pages).strip()
    
    @staticmethod
    def extract_text_docx(content : bytes) -> str:
        document = Document(BytesIO(content))
        paragraphs = [p.text for p in document.paragraphs if p.text.strip()]

        return "\n".join(paragraphs).strip()
    
    @staticmethod
    def extract_text_txt(content : bytes) -> str:
        try:
            return content.decode("utf-8").strip()
        except UnicodeDecodeError:
            return content.decode("latin-1").strip()
    
    @staticmethod
    def normalize_text(text : str) -> str:
        text = (
        unicodedata.normalize("NFD", text)
        .encode("ascii", "ignore")
        .decode("ascii")
        .lower()
        )

        return " ".join(text.split())
    
    @staticmethod
    def chunk_text(text : str,
                   tokenizer,
                   max_tokens_per_chunk : int = 500,
                   overlap : float = 1/3) -> list[TextChunk]:

        text_lenght = len(text)
        chunks : list[TextChunk] = []

        start = 0

        while start < text_lenght:
            low_bound, high_bound = start + 1, text_lenght
            last_valid_end = start
            last_valid_token_size = 0

            while low_bound <= high_bound:
                mid = (low_bound + high_bound ) // 2

                size = len(tokenizer.encode(text[start:mid]))

                if size <= max_tokens_per_chunk:
                    last_valid_end = mid
                    last_valid_token_size = size
                    low_bound = mid + 1
                else:
                    high_bound = mid - 1

            chunks.append(TextChunk(text=text[start:last_valid_end], token_size=last_valid_token_size))

            if last_valid_end >= text_lenght:
                break

            start = max(start + 1, last_valid_end - int((last_valid_end - start) * overlap) )

        return chunks

    @staticmethod
    def embed_chunks(chunks : list[TextChunk], model) -> list[EmbeddedChunk]:
        embeddings = model.encode([chunk.text for chunk in chunks], normalize_embeddings=True).tolist()
        return [EmbeddedChunk(index=i, chunk=c.text, vector=v, token_size=c.token_size) for i, (c, v) in enumerate(zip(chunks, embeddings))]