using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EnhancedMap.Core.MapObjects;
using EnhancedMap.Diagnostic;

namespace EnhancedMap.Core
{
    public static class CsvLabelManager
    {
        private static FileSystemWatcher _watcher;
        private static readonly List<LabelObject> _loadedLabels = new();
        public static Dictionary<string, List<CsvLabelObject>> LabelsByFile { get; private set; } = new();

        public static void Initialize()
        {
            CreateDefinitionsDirectory();
            LoadAllLabels();
            SetupFileWatcher();
            //File.AppendAllText("debug_label_log.txt", $"CsvLabelManager init em {DateTime.Now}\n");
        }

        private static void CreateDefinitionsDirectory()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Definitions");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, "example.csv"),
                    "X,Y,Facet,Name,Icon,Color,Zoom\n" +
                    "1697,827,0,wandering healer,healer,blue,4");
            }
        }

        private static void SetupFileWatcher()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Definitions");
            _watcher = new FileSystemWatcher(path, "*.csv")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                EnableRaisingEvents = true
            };

            _watcher.Changed += (s, e) => ReloadLabels();
            _watcher.Created += (s, e) => ReloadLabels();
            _watcher.Deleted += (s, e) => ReloadLabels();
            _watcher.Renamed += (s, e) => ReloadLabels();
        }

        public static void ReloadLabels()
        {
            lock (_loadedLabels)
            {
                foreach (var label in _loadedLabels)
                {
                    RenderObjectsManager.RemoveLabel(label);
                    label.Dispose();
                }
                _loadedLabels.Clear();
                LoadAllLabels();
                Global.MainWindow?.Invalidate();
            }
        }

        private static void LoadAllLabels()
        {
            LabelsByFile.Clear();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Definitions");

            // Lê arquivos .csv e cria labels
            foreach (var file in Directory.GetFiles(path, "*.csv"))
            {
                try
                {
                    var parsed = ParseCsvFile(file);
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    foreach (var data in parsed)
                    {
                        CreateLabelFromData(data, fileName);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error loading {file}: {ex}");
                    //File.AppendAllText("debug_label_log_ERRORS.txt", $"Error loading {file}: {ex}");

                }
            }

            // Lê arquivos .map e cria labels
            foreach (var file in Directory.GetFiles(path, "*.map"))
            {
                foreach (var data in ParseMapFile(file).Where(d => d != null))
                {
                    CreateLabelFromData(data, file);
                }
            }

        }

        private static void CreateLabelFromData(CsvLabelData data, string sourceFile)
        {
            try
            {
                //File.AppendAllText("debug_label_log.txt", $"[Create] Pos: {data.X},{data.Y} | Facet: {data.Facet} | Name: {data.Name} | Icon: {data.Icon} | Color: {data.Color} | Zoom: {data.Zoom}\n");


                //var label = new LabelObject((short)data.X, (short)data.Y, data.Name)
                //{
                //   Map = data.Facet,
                //  Hue = new SolidBrush(ColorTranslator.FromHtml(data.Color)),
                // IsVisible = true,
                //};
                var label = new CsvLabelObject((short)data.X, (short)data.Y, data.Name, data.Icon)
                {
                    Map = data.Facet,
                    Hue = new SolidBrush(ColorTranslator.FromHtml(data.Color)),
                    IsVisible = true,
                    Zoom = data.Zoom * 25 // conversão aqui

                };

                // para add as labels na nova pagina de settings
                if (!LabelsByFile.ContainsKey(sourceFile))
                    LabelsByFile[sourceFile] = new List<CsvLabelObject>();

                LabelsByFile[sourceFile].Add(label);


                _loadedLabels.Add(label);
                //File.AppendAllText("debug_label_log.txt", $"[Add] Adicionando label \"{data.Name}\" ao RenderObjectsManager\n");
                //File.AppendAllText("debug_label_log.txt", $"[DEBUG] label.IsVisible = {label.IsVisible}\n");
                //File.AppendAllText("debug_label_log.txt", $"[DEBUG] label.Map = {label.Map} | Global.Facet = {Global.Facet}\n");
                //File.AppendAllText("debug_label_log.txt", $"[DEBUG] Position X,Y: {label.Position.X},{label.Position.Y} | Relative: {label.RelativePosition.X},{label.RelativePosition.Y}\n");
                //File.AppendAllText("debug_label_log.txt", $"[DEBUG] Label '{data.Name}' Facet={data.Facet}, Label.Map={label.Map}, Global.Facet={Global.Facet}\n");
                //File.AppendAllText("debug_label_log.txt", $"[DEBUG] Criado label: {data.Name} | Zoom={data.Zoom} | Global.Zoom={Global.Zoom}\n");


                RenderObjectsManager.AddLabel(label);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error creating label at {data.X},{data.Y}: {ex}");
                //File.AppendAllText("debug_label_log_ERRORS.txt", $"[ColorError] Cor inválida \"{data.Color}\" para label \"{data.Name}\": {ex.Message}\n");

            }
        }

        private static List<CsvLabelData> ParseCsvFile(string filePath)
        {
            var results = new List<CsvLabelData>();

            try
            {
                var lines = File.ReadAllLines(filePath)
                    .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("X,Y,Facet"));

                foreach (var line in lines)
                {
                    var parts = line.Split(',').Select(p => p.Trim()).ToArray();

                    if (parts.Length < 6) continue;

                    try
                    {

                        results.Add(new CsvLabelData
                        {
                            X = int.Parse(parts[0]),
                            Y = int.Parse(parts[1]),
                            Facet = byte.Parse(parts[2]),
                            Name = parts[3].Trim('"'),
                            Icon = parts[4].Trim('"'),
                            Color = parts[5].Trim('"'),
                            Zoom = (parts.Length >= 7) ? int.Parse(parts[6]) : 0 // padrão: zoom 0 = sempre visível
                        });
                    }

                    catch (Exception ex)
                    {
                        Logger.Log($"Error parsing line: '{line}'\n{ex}");
                        //File.AppendAllText("debug_label_log_ERRORS.txt", $"[ParseError] Erro linha: '{line}'\n{ex.Message}\n");

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error reading {filePath}: {ex}");
                //File.AppendAllText("debug_label_log_ERRORS.txt", $"[ParseError 2] Erro linha: '{filePath}'\n{ex.Message}\n");

            }

            return results;
        }

        private static List<CsvLabelData> ParseMapFile(string filePath)
        {
            var results = new List<CsvLabelData>();
            var regex = new Regex(@"^([+-])([A-Z]+):\s+(\d+)\s+(\d+)\s+(\d+)\s+(.*)$");

            try
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var match = regex.Match(line);
                    if (!match.Success) continue;

                    string enabledFlag = match.Groups[1].Value;
                    string icon = match.Groups[2].Value.ToLower(); // ex: teleporter
                    int x = int.Parse(match.Groups[3].Value);
                    int y = int.Parse(match.Groups[4].Value);
                    byte facet = byte.Parse(match.Groups[5].Value);
                    string name = match.Groups[6].Value.Trim();

                    // Ignora labels desativados, se quiser
                    if (enabledFlag == "-") continue;

                    results.Add(new CsvLabelData
                    {
                        X = x,
                        Y = y,
                        Facet = facet,
                        Name = name,
                        Icon = icon,
                        Color = "yellow", // ou outro padrão
                        Zoom = 0
                    });
                }
            }
            catch (Exception ex)
            {
                //File.AppendAllText("debug_label_log_ERRORS.txt", $"[MapParseError] {filePath} => {ex.Message}\n");
            }

            return results;
        }

    }

    public class CsvLabelData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public byte Facet { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public int Zoom { get; set; }
    }
}