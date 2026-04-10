using System;
using System.Collections.Generic;
using System.Linq;

namespace CompiladorCPLUS.Service;

public class SemanticService
{
    private HashSet<string> _tablaSimbolos = new();
    private readonly string[] _tiposValidos = { "int", "float", "double", "string", "bool", "long" };

    public List<string> AnalizarSemantica(string codigo)
    {
        var errores = new List<string>();
        _tablaSimbolos.Clear();
        if (string.IsNullOrWhiteSpace(codigo)) return errores;

        string[] lineas = codigo.Split('\n');
        for (int i = 0; i < lineas.Length; i++)
        {
            int numLinea = i + 1;
            string lineaOriginal = lineas[i].Split("//")[0].Trim();
            if (string.IsNullOrEmpty(lineaOriginal) || lineaOriginal.StartsWith("#") || lineaOriginal.Contains("using") || lineaOriginal == "{" || lineaOriginal == "}") continue;

            string tipoEncontrado = _tiposValidos.FirstOrDefault(t => lineaOriginal.StartsWith(t));

            if (tipoEncontrado != null && !lineaOriginal.Contains("("))
            {
                string contenido = lineaOriginal.Substring(tipoEncontrado.Length).Replace(";", "").Trim();
                var declaraciones = contenido.Split(',');
                foreach (var decl in declaraciones)
                {
                    string nombreVar = decl.Split('=')[0].Trim();
                    if (nombreVar.Contains(" ")) nombreVar = nombreVar.Split(' ').Last();

                    if (_tablaSimbolos.Contains(nombreVar))
                        errores.Add($"[SEMÁNTICO] (Línea {numLinea}): La variable '{nombreVar}' ya ha sido declarada.");
                    else
                        _tablaSimbolos.Add(nombreVar);
                }
            }
            else if (lineaOriginal.Contains("=") && !lineaOriginal.Contains("==") && !lineaOriginal.StartsWith("for"))
            {
                string variable = lineaOriginal.Split('=')[0].Trim();
                string identificador = variable.Split(' ', '[', '.', '+', '-')[0];

                if (!string.IsNullOrEmpty(identificador) &&
                    !_tablaSimbolos.Contains(identificador) &&
                    !identificador.Contains("return") &&
                    !identificador.Contains("cout"))
                {
                    errores.Add($"[SEMÁNTICO] (Línea {numLinea}): El identificador '{identificador}' no ha sido declarado.");
                }
            }
        }
        return errores;
    }
}