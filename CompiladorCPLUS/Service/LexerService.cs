using CompiladorCPLUS.Models;
using System.Collections.Generic;

namespace CompiladorCPLUS.Service;

public class LexerService
{
    private readonly HashSet<string> _caracteresValidos = new()
    {
        "+", "-", "*", "/", "=", ">", "<", "!", ";", "{", "}", "(", ")", "\"", ".", "_", ",", "#", ":", "&", "|", "@", "'"
    };

    private readonly HashSet<string> _palabrasReservadas = new()
    {
        "asm", "auto", "bool", "break", "case", "catch", "char", "class", "const", "const_cast",
        "continue", "default", "delete", "do", "double", "dynamic_cast", "else", "enum", "explicit",
        "false", "friend", "for", "if", "inline", "int", "long", "mutable", "namespace", "new",
        "operator", "private", "protected", "public", "register", "reinterpret_cast", "return",
        "short", "signed", "sizeof", "static", "static_cast", "struct", "switch", "template",
        "this", "throw", "true", "try", "typedef", "typeid", "typename", "union", "unsigned",
        "using", "virtual", "void", "volatile", "wchar_t", "while"
    };

    public List<string> ValidarCodigo(string source)
    {
        var errores = new List<string>();
        if (string.IsNullOrEmpty(source)) return errores;

        string[] lineas = source.Split('\n');

        for (int i = 0; i < lineas.Length; i++)
        {
            string contenidoLinea = lineas[i];
            for (int j = 0; j < contenidoLinea.Length; j++)
            {
                char c = contenidoLinea[j];
                if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && !_caracteresValidos.Contains(c.ToString()))
                {
                    errores.Add($"Error Léxico (Línea {i + 1}, Columna {j + 1}): El caracter '{c}' no es reconocido.");
                }
            }
        }
        return errores;
    }

    public bool EsPalabraReservada(string palabra) => _palabrasReservadas.Contains(palabra);
}