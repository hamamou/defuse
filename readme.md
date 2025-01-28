# **defuse**

`defuse` is a command-line tool for merging multiple PDF files into a single PDF document. It provides a simple interface, robust error handling, and extensibility using Dependency Injection. Powered by the `PdfSharp` library, `defuse` is designed for developers and power users alike.

---

## **Features**

-   Merge multiple PDFs into one file.
-   Support for directories containing PDFs.

---

## **Getting Started**

### **Prerequisites**

-   [.NET 6 or later](https://dotnet.microsoft.com/download)

### **Installation**

1. Clone the repository:
    ```bash
    git clone https://github.com/hamamou/defuse.git
    cd defuse
    ```
2. Restore dependencies:

    ```bash
    dotnet restore
    ```

3. Build the project:
    ```bash
    dotnet build
    ```

---

## **Usage**

### **Basic Command**

To merge multiple PDF files:

```bash
dotnet run -- -i file1.pdf file2.pdf
```

### **Merge PDFs from a Directory**

To merge all PDF files in a directory:

```bash
dotnet run -- -i ./pdf-directory
```

### **Help**

Display help and usage examples:

```bash
dotnet run -- --help
```

### **Options**

| Option        | Description                                             | Required |
| ------------- | ------------------------------------------------------- | -------- |
| `-i, --input` | Paths to input PDF files or directories containing PDFs | Yes      |

---

## **Examples**

### Merge Two PDF Files

```bash
dotnet run -- -i "file1.pdf" "file2.pdf"
```

### Merge PDFs in a Directory

```bash
dotnet run -- -i "./documents"
```

---

## **License**

This project is licensed under the [MIT License](LICENSE).
