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

        for (int i = 0; i < lineas.Length; i++)
        {
            string linea = lineas[i].Trim();
            if (string.IsNullOrEmpty(linea) || linea.StartsWith("//") || linea.StartsWith("#") || linea.StartsWith("using")) continue;
            int numLinea = i + 1;

            bool esEstructuraValida = linea.EndsWith(";") || linea.EndsWith("{") || linea.EndsWith("}") ||
                                     linea.Contains("if") || linea.Contains("main()") || linea.Contains("else");

            if (!esEstructuraValida)
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

            int aperturas = linea.Count(f => f == '{');
            int cierres = linea.Count(f => f == '}');
            if (aperturas > 0 && cierres > 0 && linea.IndexOf("{") > linea.LastIndexOf("}"))
                erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): Orden de llaves incorrecto.");
        }
        return erroresSintacticos;
    }
}