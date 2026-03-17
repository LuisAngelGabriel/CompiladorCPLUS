using CompiladorCPLUS.Models;
using System.Text.RegularExpressions;

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
            if (string.IsNullOrEmpty(linea) || linea.StartsWith("//") || linea.StartsWith("/*")) continue;

            int numLinea = i + 1;

            // 1. Validar que toda instrucción (que no sea if/else/for/{/}) termine en punto y coma
            if (!linea.EndsWith(";") && !linea.EndsWith("{") && !linea.EndsWith("}") && !linea.Contains("if") && !linea.Contains("else"))
            {
                erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): Falta el punto y coma ';' al final de la instrucción.");
            }

            // 2. Validar estructura de asignación (ej: int x = 10;)
            if (linea.Contains("=") && !linea.Contains("if"))
            {
                // Verifica que haya algo antes y después del igual
                var partes = linea.Split('=');
                if (partes.Length < 2 || string.IsNullOrWhiteSpace(partes[0]) || string.IsNullOrWhiteSpace(partes[1].Replace(";", "")))
                {
                    erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): Asignación incompleta. Se esperaba un valor después del '='.");
                }
            }

            // 3. Validar paréntesis en estructuras de control
            if (linea.Contains("if"))
            {
                if (!linea.Contains("(") || !linea.Contains(")"))
                {
                    erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): La condición del 'if' debe estar encerrada entre paréntesis '( )'.");
                }
            }

            // 4. Validar llaves de apertura/cierre (Balance de bloques)
            // Nota: Para un análisis profundo se usaría una pila, aquí validamos por línea simple
            if (linea.Contains("{") && linea.Contains("}") && linea.IndexOf("{") > linea.IndexOf("}"))
            {
                erroresSintacticos.Add($"Error Sintáctico (Línea {numLinea}): El orden de las llaves '{{ }}' es incorrecto.");
            }
        }

        return erroresSintacticos;
    }
}