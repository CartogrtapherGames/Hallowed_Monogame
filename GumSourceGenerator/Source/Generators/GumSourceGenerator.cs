


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GumSourceGenerator.Generators;

    [Generator]
    public class GumConstantsGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Find the .gumx file
            var gumxFile = context.AdditionalFiles
                .FirstOrDefault(f => f.Path.EndsWith(".gumx", StringComparison.OrdinalIgnoreCase));

            if (gumxFile == null)
                return;

            var gumxPath = gumxFile.Path;
            var gumProjectDir = Path.GetDirectoryName(gumxPath);
            var xmlContent = gumxFile.GetText(context.CancellationToken)?.ToString();
            
            if (string.IsNullOrEmpty(xmlContent))
                return;

            try
            {
                var doc = XDocument.Parse(xmlContent);
                var root = doc.Root;

                // Get relative path to gumx
                var projectPath = MakeRelativePath(gumxPath);

                // Parse screens
                var screenNames = ParseScreenNames(root);
                
                // Parse screen files to get named instances
                var screenData = new Dictionary<string, List<string>>();
                foreach (var screenName in screenNames)
                {
                    var screenFilePath = Path.Combine(gumProjectDir, "Screens", $"{screenName}.gusx");
                    var namedInstances = ParseScreenFile(screenFilePath);
                    screenData[screenName] = namedInstances;
                }

                // Generate main constants file
                var mainSource = GenerateMainConstants(projectPath, screenNames);
                context.AddSource("GumConstants.g.cs", SourceText.From(mainSource, Encoding.UTF8));

                // Generate separate file for each screen
                foreach (var screen in screenNames)
                {
                    var screenSource = GenerateScreenConstants(screen, screenData.GetValueOrDefault(screen, new List<string>()));
                    context.AddSource($"GumConstants.{screen}.g.cs", SourceText.From(screenSource, Encoding.UTF8));
                }
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "GUM001",
                        "Gum Constants Generation Error",
                        $"Error generating Gum constants: {ex.Message}",
                        "GumGenerator",
                        DiagnosticSeverity.Warning,
                        true),
                    Location.None));
            }
        }

        private string MakeRelativePath(string path)
        {
            // Try to extract relative path from full path
            var parts = path.Replace("\\", "/").Split('/');
            var contentIndex = Array.FindLastIndex(parts, p => p.Equals("Content", StringComparison.OrdinalIgnoreCase));
            
            if (contentIndex >= 0)
            {
                return string.Join("/", parts.Skip(contentIndex));
            }
            
            return Path.GetFileName(path);
        }

        private List<string> ParseScreenNames(XElement root)
        {
            var screens = new List<string>();

            foreach (var element in root.Elements("ScreenReference"))
            {
                var name = element.Element("Name")?.Value;
                if (!string.IsNullOrEmpty(name))
                {
                    screens.Add(name);
                }
            }

            return screens;
        }

        private List<string> ParseScreenFile(string screenFilePath)
        {
            var namedInstances = new List<string>();

            if (!File.Exists(screenFilePath))
                return namedInstances;

            try
            {
                var screenXml = XDocument.Load(screenFilePath);
                var root = screenXml.Root;

                // Find all instances with names
                foreach (var instance in root.Elements("Instance"))
                {
                    var name = instance.Element("Name")?.Value;
                    if (!string.IsNullOrEmpty(name) && !name.Contains('/'))
                    {
                        namedInstances.Add(name);
                    }
                }
            }
            catch
            {
                // Ignore errors reading individual screen files
            }

            return namedInstances;
        }

        private string GenerateMainConstants(string gumxPath, List<string> screens)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("// Main Gum constants");
            sb.AppendLine();
            sb.AppendLine("namespace YourGame.Gum;");
            sb.AppendLine();
            sb.AppendLine("public static class GumConstants");
            sb.AppendLine("{");
            
            // Gum project path
            sb.AppendLine($"    public const string GumProjectPath = \"{gumxPath}\";");
            sb.AppendLine();

            // Screen name constants
            sb.AppendLine("    // Screen Names");
            foreach (var screen in screens)
            {
                sb.AppendLine($"    public const string {MakeSafeIdentifier(screen)} = \"{screen}\";");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateScreenConstants(string screenName, List<string> instances)
        {
            var sb = new StringBuilder();
            var className = MakeSafeIdentifier(screenName);

            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine($"// Constants for {screenName}");
            sb.AppendLine();
            sb.AppendLine("namespace YourGame.Gum;");
            sb.AppendLine();
            sb.AppendLine($"public static class {className}");
            sb.AppendLine("{");
            
            // Screen name
            sb.AppendLine($"    public const string ScreenName = \"{screenName}\";");

            if (instances.Any())
            {
                sb.AppendLine();
                sb.AppendLine("    // Named Instances");
                
                foreach (var instance in instances)
                {
                    var safeName = MakeSafeIdentifier(instance);
                    sb.AppendLine($"    public const string {safeName} = \"{instance}\";");
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        private string MakeSafeIdentifier(string name)
        {
            // Remove invalid characters
            var safe = new string(name.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
            
            if (string.IsNullOrEmpty(safe) || char.IsDigit(safe[0]))
            {
                safe = "_" + safe;
            }

            return safe;
        }
    }
