using System;
using System.Collections.Generic;
using System.Linq;

namespace CompiladorCPLUS.Service;

public class SemanticService
{
    private Dictionary<string, string> _tablaTipos = new();
    private Dictionary<string, string> _tablaValores = new();
    private readonly string[] _tiposValidos = { "int", "float", "double", "string", "bool", "char", "long", "short", "unsigned" };

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
            if (string.IsNullOrEmpty(linea) || linea.StartsWith("//") || linea.StartsWith("#") || linea.StartsWith("using") || linea == "{" || linea == "}") continue;
            int numLinea = i + 1;

            if (_tiposValidos.Any(t => linea.StartsWith(t)))
            {
                var partesDeclaracion = linea.Replace(";", "").Split('=')[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string nombreVar = partesDeclaracion.Last();
                string tipo = partesDeclaracion[0];

                if (_tablaTipos.ContainsKey(nombreVar))
                    errores.Add($"Error Semántico (Línea {numLinea}): La variable '{nombreVar}' ya fue declarada.");
                else
                    _tablaTipos.Add(nombreVar, tipo);
            }
            else if (linea.Contains("=") && !linea.Contains("if") && !linea.Contains("=="))
            {
                var varNombreRaw = linea.Split('=')[0].Trim();
                var busquedaVar = varNombreRaw.Contains(".") ? varNombreRaw.Split('.')[0] : varNombreRaw;

                if (!_tablaTipos.ContainsKey(busquedaVar) && !varNombreRaw.Contains("static_cast"))
                {
                    errores.Add($"Error Semántico (Línea {numLinea}): La variable '{varNombreRaw}' no existe.");
                }
            }
        }
        return errores;
    }

    public void ActualizarValorDesdeInput(string nombre, string valor)
    {
        if (_tablaTipos.ContainsKey(nombre)) _tablaValores[nombre] = valor;
    }

    public Dictionary<string, string> ObtenerTablaSimbolos() => _tablaTipos;
    public Dictionary<string, string> ObtenerValores() => _tablaValores;
}