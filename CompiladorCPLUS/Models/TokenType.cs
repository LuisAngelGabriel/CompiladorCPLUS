namespace CompiladorCPLUS.Models
{
    public enum TokenType
    {
        // Palabras reservadas (Control de if, else, bucles)
        Keyword,
        // Nombres de variables o funciones
        Identifier,
        // Tipos de datos y valores
        Number,
        String,
        // Operaciones (Aritméticas y lógicas)
        Operator,
        // Símbolos de estructura (Control de punto y coma, llaves)
        Separator,
        // Control de errores (Cuando algo está mal escrito)
        Error,
        // Fin de archivo
        EOF
    }
}
