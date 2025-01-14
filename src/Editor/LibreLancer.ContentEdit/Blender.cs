using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using LibreLancer.Data;
using SimpleMesh;

namespace LibreLancer.ContentEdit;

public class Blender
{
    private static readonly string ExportScript;

    static Blender()
    {
        var reader = new StreamReader(typeof(Blender).Assembly.GetManifestResourceStream("LibreLancer.ContentEdit.BlenderExport.py")!);
        ExportScript = reader.ReadToEnd();
    }

    public static string AutodetectBlender()
    {
        if (Platform.RunningOS == OS.Windows)
        {
            //Try and get highest blender version from Program Files
            var blenderDefaultDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Blender Foundation");
            if (!Directory.Exists(blenderDefaultDir)) return null;
            var blenderDir = Directory.GetDirectories(blenderDefaultDir).Where(x => x.StartsWith("Blender ")).MaxBy(x => x);
            if (blenderDir == null) return null;
            if (File.Exists(Path.Combine(blenderDir, "blender.exe")))
                return Path.Combine(blenderDir, "blender.exe");
            return null;
        }
        if (Shell.HasCommand("blender")) return "blender";
        if (Shell.HasCommand("flatpak"))
        {
            //Load from flatpak
            var installed = Shell.GetString("flatpak", "list --columns=application");
            if (installed.Contains("org.blender.Blender"))
                return "FLATPAK";
        }
        return null;
    }

    public static bool FileIsBlender(string filename)
    {
        using var stream = File.OpenRead(filename);
        return FileIsBlender(stream);
    }

    public static bool FileIsBlender(Stream stream)
    {
        Span<byte> bytes = stackalloc byte[9];
        if (stream.Read(bytes) != 9) return false;
        if (bytes[0] != 0x42 || //BLENDER magic
            bytes[1] != 0x4C ||
            bytes[2] != 0x45 ||
            bytes[3] != 0x4E ||
            bytes[4] != 0x44 ||
            bytes[5] != 0x45 ||
            bytes[6] != 0x52)
            return false;
        if (bytes[7] != 0x2D && bytes[7] != 0x5F) //- or _ pointer size
            return false;
        if (bytes[8] != 0x76 && bytes[7] != 0x56) //v or V endianness
            return false;
        return true;
    }

    public static bool BlenderPathValid(string blenderPath = null)
    {
        if (string.IsNullOrWhiteSpace(blenderPath))
            blenderPath = AutodetectBlender();
        if (string.IsNullOrWhiteSpace(blenderPath))
            return false;
        if (blenderPath == "FLATPAK")
            return Shell.GetString("flatpak", "run org.blender.Blender --version", 10000)
                .StartsWith("Blender");
        return Shell.GetString(blenderPath, "--version", 10000)
                .StartsWith("Blender");
    }

    static string EscapeCode(string s) => JsonValue.Create(s).ToJsonString();

    private const int CANCELLED = -255;
    static async Task<int> RunBlender(string blenderPath, string args, string pythonCode, CancellationToken cancellation = default, Action<string> log = null)
    {
        var processName = blenderPath;
        var processArgs = $"{args} --background --factory-startup --python-console";
        if (blenderPath == "FLATPAK")
        {
            processName = "flatpak";
            processArgs = $"run --filesystem=/tmp org.blender.Blender {processArgs}";
        }

        var psi = new ProcessStartInfo(processName, processArgs)
        {
            UseShellExecute = false,
            RedirectStandardOutput = log != null,
            RedirectStandardError = log != null,
            RedirectStandardInput = true
        };
        log?.Invoke($"Running {processName} {processArgs}\n");
        log?.Invoke("Python:\n" + pythonCode + "\n");
        var process = Process.Start(psi);
        if (log != null)
        {
            process.OutputDataReceived += (o, e) =>
            {
                if (e.Data != null) log(e.Data + "\n");
            };
            process.ErrorDataReceived += (o, e) =>
            {
                if (e.Data != null) log(e.Data + "\n");
            };
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }
        await process.StandardInput.WriteAsync(pythonCode);
        process.StandardInput.Close();
        try
        {
            await process.WaitForExitAsync(cancellation);
        }
        catch (TaskCanceledException)
        {
            process.CancelErrorRead();
            process.CancelOutputRead();
            process.TryKill();
            return CANCELLED;
        }
        return process.ExitCode;
    }

    static void DeleteIfExists(string file)
    {
        if(!string.IsNullOrEmpty(file) && File.Exists(file))
            File.Delete(file);
    }

    public static async Task<EditResult<SimpleMesh.Model>> LoadBlenderFile(string file, CancellationToken cancellation = default, Action<string> log = null, string blenderPath = null)
    {
        if (string.IsNullOrWhiteSpace(blenderPath))
            blenderPath = AutodetectBlender();
        if (string.IsNullOrWhiteSpace(blenderPath))
            return EditResult<SimpleMesh.Model>.Error("Could not locate blender executable");
        string tmpblend = null;
        string tmpfile = Path.GetTempFileName();
        File.Delete(tmpfile);
        if (blenderPath == "FLATPAK")
        {
            tmpblend = Path.GetTempFileName();
            File.Copy(file, tmpblend, true);
        }
        var exportCode =
            "import bpy\n"
            + $"bpy.ops.export_scene.gltf(filepath={EscapeCode(tmpfile)}, export_format='GLB', export_extras=True, use_mesh_edges=True, export_image_format='AUTO')";
        var result = await RunBlender(blenderPath, Shell.Quote(tmpblend ?? file), exportCode, cancellation, log);
        if (result == CANCELLED)
        {
            DeleteIfExists(tmpblend);
            DeleteIfExists(tmpfile);
            return EditResult<SimpleMesh.Model>.Error("Operation was cancelled");
        }
        log?.Invoke($"Exit Code: {result}\n");
        DeleteIfExists(tmpblend);
        tmpfile += ".glb";
        if(File.Exists(tmpfile))
        {
            return await EditResult<SimpleMesh.Model>.RunBackground(() =>
            {
                var bytes = File.ReadAllBytes(tmpfile);
                File.Delete(tmpfile);
                using var ms = new MemoryStream(bytes);
                return EditResult<SimpleMesh.Model>.TryCatch(() => SimpleMesh.Model.FromStream(ms));
            }, cancellation);
        }
        return EditResult<SimpleMesh.Model>.Error("Failed to execute blender export");
    }

    const float RADIUS_DIVIDER = 21.916825f;

    static void CalcMinMax(ModelNode n, ref Vector3 min, ref Vector3 max, Matrix4x4 parent)
    {
        if (n.Geometry != null)
        {
            var modelMin = Vector3.Transform(n.Geometry.Min, n.Transform * parent);
            var modelMax = Vector3.Transform(n.Geometry.Max, n.Transform * parent);
            min = Vector3.Min(modelMin, min);
            max = Vector3.Max(modelMax, max);
        }
        foreach (var c in n.Children) {
            CalcMinMax(c, ref min, ref max, n.Transform * parent);
        }
    }

    public static async Task<EditResult<bool>> ExportBlenderFile(SimpleMesh.Model exported, string file, string blenderPath = null, CancellationToken cancellation = default, Action<string> logLine = null)
    {
        if (string.IsNullOrWhiteSpace(blenderPath))
            blenderPath = AutodetectBlender();
        if (string.IsNullOrWhiteSpace(blenderPath))
            return EditResult<bool>.Error("Could not locate blender executable");
        var tmpblend = Path.GetTempFileName();
        File.Delete(tmpblend);
        var tmpfile = Path.GetTempFileName();
        using (var gltfStream = File.Create(tmpfile)) {
            exported.SaveTo(gltfStream, ModelSaveFormat.GLTF2);
        }

        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        exported.CalculateBounds();
        foreach (var x in exported.Roots) {
            CalcMinMax(x, ref min, ref max, Matrix4x4.Identity);
        }

        var radius = (max - min).Length() / 2.0f;
        float hpScale = radius / RADIUS_DIVIDER;

        var result = await RunBlender(
            blenderPath,
            "",
            string.Format(ExportScript, EscapeCode(tmpfile), EscapeCode(tmpblend), hpScale.ToStringInvariant()),
            cancellation,
            logLine
        );
        if(result != CANCELLED)
            logLine?.Invoke($"Exit Code: {result}\n");
        File.Delete(tmpfile);
        if (File.Exists(tmpblend))
        {
            File.Move(tmpblend, file, true);
            return true.AsResult();
        }
        else
        {
            return false.AsResult();
        }
    }
}
