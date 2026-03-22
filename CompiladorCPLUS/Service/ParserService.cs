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

        for (int i = 0; i < lineas.Length; i++)
        {
            string linea = lineas[i].Trim();
            if (string.IsNullOrEmpty(linea) || linea.StartsWith("//") || linea.StartsWith("#") || linea.StartsWith("using")) continue;
            int numLinea = i + 1;

            balanceLlaves += linea.Count(f => f == '{');
            balanceLlaves -= linea.Count(f => f == '}');

            bool esEstructuraOmitida = linea.EndsWith("{") || linea.EndsWith("}") || linea.EndsWith(":") ||
                                     linea.Contains("if") || linea.Contains("else") ||
                                     linea.Contains("main()") || linea.Contains("for") ||
                                     linea.Contains("while") || linea.Contains("switch");

            if (!linea.EndsWith(";") && !esEstructuraOmitida)
            {
                erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): Falta ';' al final de la instrucción.");
            }

            if (linea.Contains("cout") || linea.Contains("cin") || linea.Contains("print") || linea.Contains("input"))
            {
                if ((linea.Contains("print") || linea.Contains("input")) && (!linea.Contains("(") || !linea.Contains(")")))
                    erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): La función requiere '( )'.");

                if (linea.Contains("cout") && !linea.Contains("<<"))
                    erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): 'cout' requiere el operador '<<'.");

                if (linea.Contains("cin") && !linea.Contains(">>"))
                    erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): 'cin' requiere el operador '>>'.");
            }
        }

        if (balanceLlaves != 0)
            erroresSintacticos.Add("Error Sintáctico: El número de llaves '{' y '}' no coincide.");

        return erroresSintacticos;
    }
}