using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Aga.Controls.Tree;
using EnhancedMap.Core;
using EnhancedMap.Core.MapObjects;
using static System.Windows.Forms.AxHost;

namespace EnhancedMap.GUI.SettingsLayouts
{
    public partial class CsvLabelsLayout : UserControl
    {
        private readonly TreeModel _model = new TreeModel();

        public CsvLabelsLayout()
        {
            InitializeComponent(); // <-- ESSENCIAL
            treeViewAdv1.Model = _model;

            customButtonExpandAll.Click += (s, e) => treeViewAdv1.ExpandAll();
            customButtonCollapseAll.Click += (s, e) => treeViewAdv1.CollapseAll();
            customButtonEnableAll.Click += (s, e) =>
            {
                foreach (CsvLabelNode root in _model.Nodes.OfType<CsvLabelNode>())
                {
                    root.State = CheckState.Checked;
                }
                treeViewAdv1.Invalidate();
            };
            customButtonDisableAll.Click += (s, e) =>
            {
                foreach (CsvLabelNode root in _model.Nodes.OfType<CsvLabelNode>())
                {
                    root.State = CheckState.Unchecked;
                    foreach (CsvLabelNode node in root.Nodes.OfType<CsvLabelNode>())
                        node.State = CheckState.Unchecked;
                }
                treeViewAdv1.Invalidate();
            };
        }

        public void BuildList(Dictionary<string, List<CsvLabelObject>> groupedLabels)
        {
            _model.Nodes.Clear();
            treeViewAdv1.BeginUpdate();

            foreach (var kvp in groupedLabels)
            {
                string fileName = kvp.Key;
                List<CsvLabelObject> labels = kvp.Value;

                // Cria nó de grupo com o nome do arquivo, sem label vinculado
                var root = new CsvLabelNode(null)
                {
                    DisplayName = fileName,
                    IsGroup = true,
                    State = CheckState.Indeterminate
                };

                _model.Nodes.Add(root);

                foreach (var label in labels)
                {
                    var node = new CsvLabelNode(label)
                    {
                        State = label.IsVisible ? CheckState.Checked : CheckState.Unchecked
                    };
                    root.Nodes.Add(node);
                }
            }

            treeViewAdv1.EndUpdate();
            treeViewAdv1.ExpandAll();
        }



        private class CsvLabelNode : Node
        {
            private CheckState _state;
            private readonly CsvLabelObject _label;
            public Image IconImage => ResizeIcon(_label?.GetIconBitmap());

            public CsvLabelNode(CsvLabelObject label)
            {
                _label = label;
                Tag = label;
                if (_label != null)
                {
                    DisplayName = _label.Text;
                    _state = _label.IsVisible ? CheckState.Checked : CheckState.Unchecked;
                }
            }

            public string DisplayName { get; set; }
            public bool IsGroup { get; set; }

            public string Name => _label?.Text ?? DisplayName;
            public int X => _label?.X ?? 0;
            public int Y => _label?.Y ?? 0;
            public byte Facet => _label?.Facet ?? 0;
            public string Zoom => _label?.Zoom.ToString() ?? "";

            public CheckState State
            {
                get => _state;
                set
                {
                    _state = value;

                    if (IsGroup)
                    {
                        // Propaga para os filhos
                        foreach (CsvLabelNode child in Nodes)
                            child.State = value;
                    }
                    else if (_label != null)
                    {
                        _label.IsVisible = value == CheckState.Checked;
                    }
                }
            }


            private Image ResizeIcon(Image original)
            {
                if (original == null)
                    return null;

                try
                {
                    var bmp = new Bitmap(16, 16); // tamanho fixo
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(original, 0, 0, 16, 16);
                    }
                    return bmp;
                }
                catch
                {
                    return null;
                }
            }
        }


    }
}
