using System.Collections.Generic;
using System.Linq;

namespace CompiladorCPLUS.Service;

public class SemanticService
{
    private Dictionary<string, string> _tablaTipos = new();
    private Dictionary<string, string> _tablaValores = new();

    public List<string> AnalizarSemantica(string codigo)
    {
        var errores = new List<string>();
        _tablaTipos = new Dictionary<string, string>();
        _tablaValores = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(codigo)) return errores;

        string[] lineas = codigo.Split('\n');

        for (int i = 0; i < lineas.Length; i++)
        {
            string linea = lineas[i].Trim();
            if (string.IsNullOrEmpty(linea) || linea.StartsWith("//") || linea.StartsWith("/*")) continue;
            int numLinea = i + 1;

            if (linea.StartsWith("int ") || linea.StartsWith("float ") || linea.StartsWith("string "))
            {
                var partes = linea.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length < 2) continue;

                string tipo = partes[0];
                string resto = string.Join(" ", partes.Skip(1)).Replace(";", "");
                var asignacion = resto.Split('=');
                string nombreVar = asignacion[0].Trim();

                if (_tablaTipos.ContainsKey(nombreVar))
                {
                    errores.Add($"Error Semántico (Línea {numLinea}): La variable '{nombreVar}' ya fue declarada.");
                }
                else
                {
                    _tablaTipos.Add(nombreVar, tipo);
                    if (asignacion.Length > 1)
                    {
                        string valor = asignacion[1].Trim();
                        if (!valor.Contains("input("))
                        {
                            ValidarCoherenciaTipo(tipo, valor, numLinea, errores);
                            _tablaValores[nombreVar] = valor.Replace("\"", "");
                        }
                    }
                }
            }
            else if (linea.Contains("=") && !linea.Contains("if") && !linea.Contains("input(") && !linea.Contains("=="))
            {
                var partes = linea.Split('=');
                string nombreVar = partes[0].Trim();
                string valor = partes[1].Replace(";", "").Trim();

                if (!_tablaTipos.ContainsKey(nombreVar))
                {
                    errores.Add($"Error Semántico (Línea {numLinea}): La variable '{nombreVar}' no existe.");
                }
                else
                {
                    ValidarCoherenciaTipo(_tablaTipos[nombreVar], valor, numLinea, errores);
                    _tablaValores[nombreVar] = valor.Replace("\"", "");
                }
            }
            else if (linea.Contains("input("))
            {
                var varName = linea.Split('=')[0].Replace("string", "").Replace("int", "").Replace("float", "").Trim();
                if (!_tablaTipos.ContainsKey(varName))
                {
                    errores.Add($"Error Semántico (Línea {numLinea}): La variable '{varName}' debe declararse antes de usarse con input.");
                }
            }
        }
        return errores;
    }

    private void ValidarCoherenciaTipo(string tipo, string valor, int linea, List<string> errores)
    {
        if (tipo == "int" && (valor.Contains(".") || valor.Contains("\"")))
            errores.Add($"Error de Tipo (Línea {linea}): No se puede asignar a 'int'.");

        if (tipo == "string" && !valor.StartsWith("\"") && !_tablaTipos.ContainsKey(valor))
            errores.Add($"Error de Tipo (Línea {linea}): Se esperaba comillas.");
    }

    public void ActualizarValorDesdeInput(string nombre, string valor)
    {
        if (_tablaTipos.ContainsKey(nombre)) _tablaValores[nombre] = valor;
    }

    public Dictionary<string, string> ObtenerTablaSimbolos() => _tablaTipos;
    public Dictionary<string, string> ObtenerValores() => _tablaValores;
}