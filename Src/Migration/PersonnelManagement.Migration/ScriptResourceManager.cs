namespace PersonnelManagement.Migration;

public class ScriptResourceManager
{
    public string Read(string name, string? resourcesBasePath = null)
    {
        var assembly = typeof(ScriptResourceManager).Assembly;
        if (resourcesBasePath == null)
            resourcesBasePath = typeof(ScriptResourceManager).Namespace;

        var resoucePath = $"{resourcesBasePath}.{name}";
        using var stream = assembly.GetManifestResourceStream(resoucePath);

        if (stream == null)
            throw new Exception(message: "Stream is Null");

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}