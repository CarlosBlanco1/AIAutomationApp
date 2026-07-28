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
        diacritic_removed_text = unicodedata.normalize('NFD', text).encode('ascii', 'ignore')
    
        lower_cased_text = diacritic_removed_text.lower()

        text_as_itierable = lower_cased_text.split()
        whitespace_removed_text = []

        for word in text_as_itierable:
            if word.isspace():
                continue
            else:
                whitespace_removed_text.append(word.strip())

        return " ".join(whitespace_removed_text)
    
    @staticmethod
    def chunk_text(text : str,
                   tokenizer,
                   max_tokens_per_chunk : int = 500,
                   overlap : float = 1/3) -> list[str]:

        text_lenght = len(text)
        chunks = []

        start = 0

        while start < text_lenght:
            low_bound, high_bound = start, text_lenght
            last_valid_end = start + 1

            while low_bound < high_bound:
                mid = (low_bound + high_bound ) // 2

                size = len(tokenizer.encode(text[start:mid]))

                if size < max_tokens_per_chunk:
                    low_bound = mid + 1
                    last_valid_end = mid
                else:
                    high_bound = mid - 1

            chunks.append(text[start:last_valid_end])

            if last_valid_end >= text_lenght:
                break

            start = max(start + 1, int((last_valid_end - start) * overlap) )

        return chunks

    @staticmethod
    def embed_chunks(chunks : list[str], model) -> list[EmbeddedChunk]:
        embeddings = model.encode(chunks, normalize_embeddings=True).tolist()
        return [EmbeddedChunk(index=i, chunk=c, vector=v) for i, (c, v) in enumerate(zip(chunks, embeddings))]