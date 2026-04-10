using CompiladorCPLUS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CompiladorCPLUS.Service;

public class ParserService
{
    public List<string> AnalizarSintaxis(string codigo)
    {
        var erroresSintacticos = new List<string>();
        if (string.IsNullOrWhiteSpace(codigo)) return erroresSintacticos;

        string[] lineas = codigo.Split('\n');
        int balanceLlaves = 0;
        int balanceParentesis = 0;

        for (int i = 0; i < lineas.Length; i++)
        {
            string lineaOriginal = lineas[i].Trim();
            string linea = lineaOriginal.Split("//")[0].Trim();

            if (string.IsNullOrEmpty(linea) || linea.StartsWith("#") || linea.StartsWith("using")) continue;
            int numLinea = i + 1;

            balanceLlaves += linea.Count(f => f == '{');
            balanceLlaves -= linea.Count(f => f == '}');
            balanceParentesis += linea.Count(f => f == '(');
            balanceParentesis -= linea.Count(f => f == ')');

            bool esEstructuraOmitida = linea.EndsWith("{") || linea.EndsWith("}") || linea.EndsWith(":") ||
                                     linea.Contains("if") || linea.Contains("else") ||
                                     linea.Contains("main()") || linea.Contains("for") ||
                                     linea.Contains("while") || linea.Contains("switch");

            if (!linea.EndsWith(";") && !esEstructuraOmitida && !string.IsNullOrEmpty(linea))
            {
                erroresSintacticos.Add($"[SINTÁCTICO] (Línea {numLinea}): Falta ';' al final de la instrucción.");
            }

            if ((linea.Contains("if") || linea.Contains("while") || linea.Contains("for")) && (!linea.Contains("(") || !linea.Contains(")")))
            {
                erroresSintacticos.Add($"[SINTÁCTICO] (Línea {numLinea}): Estructura de control requiere '(' y ')'.");
            }
        }

        if (balanceLlaves != 0) erroresSintacticos.Add("[SINTÁCTICO] El número de llaves '{' y '}' no coincide.");
        if (balanceParentesis != 0) erroresSintacticos.Add("[SINTÁCTICO] El número de paréntesis '(' y ')' no coincide.");

        return erroresSintacticos;
    }
}