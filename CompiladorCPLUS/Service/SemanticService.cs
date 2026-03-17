using System.Collections.Generic;
using System.Linq;

namespace CompiladorCPLUS.Service;

public class SemanticService
{
    // Esta tabla almacena el estado de la última compilación para mostrarla en la UI
    private Dictionary<string, string> _ultimaTablaSimbolos = new();

    public List<string> AnalizarSemantica(string codigo)
    {
        var errores = new List<string>();
        _ultimaTablaSimbolos = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(codigo)) return errores;

        string[] lineas = codigo.Split('\n');

        for (int i = 0; i < lineas.Length; i++)
        {
            string linea = lineas[i].Trim();
            if (string.IsNullOrEmpty(linea) || linea.StartsWith("//") || linea.StartsWith("/*")) continue;
            int numLinea = i + 1;

            // 1. DETECCIÓN DE DECLARACIONES (int x = 10;)
            if (linea.StartsWith("int ") || linea.StartsWith("float ") || linea.StartsWith("string "))
            {
                var partes = linea.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length < 2) continue;

                string tipo = partes[0]; // int, float o string
                string resto = string.Join(" ", partes.Skip(1)).Replace(";", "");

                var asignacion = resto.Split('=');
                string nombreVar = asignacion[0].Trim();

                // Regla: No duplicar nombres de variables
                if (_ultimaTablaSimbolos.ContainsKey(nombreVar))
                {
                    errores.Add($"Error Semántico (Línea {numLinea}): La variable '{nombreVar}' ya fue declarada en este ámbito.");
                }
                else
                {
                    _ultimaTablaSimbolos.Add(nombreVar, tipo);
                }

                // Validación de tipo en la asignación inmediata
                if (asignacion.Length > 1)
                {
                    string valor = asignacion[1].Trim();
                    ValidarCoherenciaTipo(tipo, valor, numLinea, errores);
                }
            }
            // 2. DETECCIÓN DE USO/ASIGNACIÓN (x = 20;)
            else if (linea.Contains("=") && !linea.Contains("if") && !linea.Contains("while"))
            {
                var partes = linea.Split('=');
                string nombreVar = partes[0].Trim();
                string valor = partes[1].Replace(";", "").Trim();

                // Regla: No puedes usar lo que no existe
                if (!_ultimaTablaSimbolos.ContainsKey(nombreVar))
                {
                    errores.Add($"Error Semántico (Línea {numLinea}): El identificador '{nombreVar}' no existe en el contexto actual.");
                }
                else
                {
                    string tipoRegistrado = _ultimaTablaSimbolos[nombreVar];
                    ValidarCoherenciaTipo(tipoRegistrado, valor, numLinea, errores);
                }
            }
        }
        return errores;
    }

    private void ValidarCoherenciaTipo(string tipo, string valor, int linea, List<string> errores)
    {
        // Validación para Enteros
        if (tipo == "int")
        {
            if (valor.Contains(".") || valor.Contains("\""))
            {
                errores.Add($"Error de Tipo (Línea {linea}): Conflicto de tipos. No se puede convertir 'Literal' a 'int'.");
            }
            else if (!int.TryParse(valor, out _) && !EsNombreVariableValido(valor))
            {
                errores.Add($"Error de Tipo (Línea {linea}): El valor asignado a '{tipo}' no es un formato numérico válido.");
            }
        }

        // Validación para Strings
        if (tipo == "string" && !valor.StartsWith("\""))
        {
            // Si no empieza con comilla y no es otra variable existente
            if (!_ultimaTablaSimbolos.ContainsKey(valor))
            {
                errores.Add($"Error de Tipo (Línea {linea}): Se esperaba una cadena literal (entre comillas) para el tipo 'string'.");
            }
        }
    }

    private bool EsNombreVariableValido(string valor)
    {
        return _ultimaTablaSimbolos.ContainsKey(valor);
    }

    // MÉTODO CLAVE: Permite que el CompiladorPage obtenga los datos para la tabla de la derecha
    public Dictionary<string, string> ObtenerTablaSimbolos(string codigo)
    {
        // Si la tabla está vacía, hacemos un análisis rápido para poblarla
        if (_ultimaTablaSimbolos.Count == 0 && !string.IsNullOrEmpty(codigo))
        {
            AnalizarSemantica(codigo);
        }
        return _ultimaTablaSimbolos;
    }
}