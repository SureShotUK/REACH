class Tools:
    def write_file(self, filename: str, content: str) -> str:
        """
        Write content to a file. Use this to save documents, notes, or structured data.
        :param filename: Filename including extension, e.g. "report.md" or "data.json". No path separators allowed.
        :param content: The full text content to write into the file.
        :return: Confirmation message with the file path written.
        """
        import os

        upload_dir = "/mnt/uploads"
        os.makedirs(upload_dir, exist_ok=True)

        # Strip any path components — filename only
        safe_name = os.path.basename(filename)
        if not safe_name:
            return "Error: Invalid filename."

        filepath = os.path.join(upload_dir, safe_name)

        with open(filepath, "w", encoding="utf-8") as f:
            f.write(content)

        return f"File written successfully: {filepath}"
