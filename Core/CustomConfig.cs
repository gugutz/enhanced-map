using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EnhancedMap.Core
{
    public class CustomConfigs
    {
        private const string ConfigFilePath = "custom_config.json"; // Caminho do arquivo JSON de configuração

        public Dictionary<string, LabelConfig> Labels { get; set; }

        public CustomConfigs()
        {
            Labels = new Dictionary<string, LabelConfig>();
            LoadConfig(); // Carregar configuração no início
        }

        // Método para carregar a configuração
        public void LoadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllText(ConfigFilePath);
                var config = JsonConvert.DeserializeObject<CustomConfigs>(json);
                Labels = config?.Labels ?? new Dictionary<string, LabelConfig>();
            }
        }

        // Método para salvar a configuração
        public void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }

        // Método para adicionar ou atualizar a configuração de uma label
        public void SetLabelState(string labelId, bool isVisible)
        {
            if (!Labels.ContainsKey(labelId))
            {
                Labels[labelId] = new LabelConfig { LabelId = labelId, IsVisible = isVisible };
            }
            else
            {
                Labels[labelId].IsVisible = isVisible;
            }

            SaveConfig(); // Salva a configuração após qualquer alteração
        }

        // Método para obter o estado de uma label
        public bool GetLabelState(string labelId)
        {
            return Labels.ContainsKey(labelId) ? Labels[labelId].IsVisible : false;
        }

        // Classe para representar a configuração de uma label
        public class LabelConfig
        {
            public string LabelId { get; set; } // ID único da label (nome + x + y + facet)
            public bool IsVisible { get; set; }
        }
    }
}
