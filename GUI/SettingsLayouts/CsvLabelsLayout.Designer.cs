namespace EnhancedMap.GUI.SettingsLayouts
{
    partial class CsvLabelsLayout
    {
        private Aga.Controls.Tree.TreeViewAdv treeViewAdv1;
        private Aga.Controls.Tree.TreeColumn treeColumnName;
        private Aga.Controls.Tree.TreeColumn treeColumnX;
        private Aga.Controls.Tree.TreeColumn treeColumnY;
        private Aga.Controls.Tree.TreeColumn treeColumnFacet;
        private Aga.Controls.Tree.TreeColumn treeColumnZoom;
        private Aga.Controls.Tree.TreeColumn treeColumnIcon;

        private Aga.Controls.Tree.NodeControls.NodeCheckBox nodeCheckBox;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxName;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxX;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxY;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxFacet;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBoxZoom;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon;


        private CustomButton customButtonExpandAll;
        private CustomButton customButtonCollapseAll;
        private CustomButton customButtonEnableAll;
        private CustomButton customButtonDisableAll;

        private void InitializeComponent()
        {
            this.treeViewAdv1 = new Aga.Controls.Tree.TreeViewAdv();
            this.treeColumnName = new Aga.Controls.Tree.TreeColumn()
            {
                Header = "Name",
                Width = 220 // ou o valor que achar melhor
            };
            this.treeColumnX = new Aga.Controls.Tree.TreeColumn() { Header = "X" };
            this.treeColumnY = new Aga.Controls.Tree.TreeColumn() { Header = "Y" };
            this.treeColumnFacet = new Aga.Controls.Tree.TreeColumn() { Header = "Facet" };
            this.treeColumnZoom = new Aga.Controls.Tree.TreeColumn() { Header = "Zoom" };
            this.treeColumnIcon = new Aga.Controls.Tree.TreeColumn()
            {
                Header = "Icon",
                Width = 40 // ajustável conforme o tamanho do ícone
            };



            this.nodeCheckBox = new Aga.Controls.Tree.NodeControls.NodeCheckBox
            {
                DataPropertyName = "State",
                EditEnabled = true,
                ThreeState = false,
                ParentColumn = this.treeColumnName
            }; 
            this.nodeTextBoxName = new Aga.Controls.Tree.NodeControls.NodeTextBox { DataPropertyName = "Name", ParentColumn = this.treeColumnName };
            this.nodeTextBoxX = new Aga.Controls.Tree.NodeControls.NodeTextBox { DataPropertyName = "X", ParentColumn = this.treeColumnX };
            this.nodeTextBoxY = new Aga.Controls.Tree.NodeControls.NodeTextBox { DataPropertyName = "Y", ParentColumn = this.treeColumnY };
            this.nodeTextBoxFacet = new Aga.Controls.Tree.NodeControls.NodeTextBox { DataPropertyName = "Facet", ParentColumn = this.treeColumnFacet };
            this.treeColumnZoom = new Aga.Controls.Tree.TreeColumn { Header = "Zoom", Width = 50 };
            this.nodeTextBoxZoom = new Aga.Controls.Tree.NodeControls.NodeTextBox
            {
                DataPropertyName = "Zoom",
                ParentColumn = this.treeColumnZoom
            };
            this.nodeIcon = new Aga.Controls.Tree.NodeControls.NodeIcon
            {
                DataPropertyName = "IconImage",
                ParentColumn = this.treeColumnIcon,
                ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip // ou None
            };



            this.customButtonExpandAll = new CustomButton { Text = "Expand All", Location = new System.Drawing.Point(10, 420) };
            this.customButtonCollapseAll = new CustomButton { Text = "Collapse All", Location = new System.Drawing.Point(120, 420) };
            this.customButtonEnableAll = new CustomButton { Text = "Enable All", Location = new System.Drawing.Point(230, 420) };
            this.customButtonDisableAll = new CustomButton { Text = "Disable All", Location = new System.Drawing.Point(340, 420) };

            // Columns
            this.treeViewAdv1.Columns.Add(this.treeColumnName);
            this.treeViewAdv1.Columns.Add(this.treeColumnX);
            this.treeViewAdv1.Columns.Add(this.treeColumnY);
            this.treeViewAdv1.Columns.Add(this.treeColumnFacet);
            this.treeViewAdv1.Columns.Add(this.treeColumnZoom);
            this.treeViewAdv1.Columns.Add(this.treeColumnIcon); // 👈 por último



            // NodeControls
            this.treeViewAdv1.NodeControls.Add(this.nodeCheckBox);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxName);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxX);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxY);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxFacet);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBoxZoom);
            this.treeViewAdv1.NodeControls.Add(this.nodeIcon); // 👈 por último


            this.treeViewAdv1.Location = new System.Drawing.Point(10, 10);
            this.treeViewAdv1.Size = new System.Drawing.Size(480, 400);
            this.treeViewAdv1.UseColumns = true;

            this.Controls.Add(this.treeViewAdv1);
            this.Controls.Add(this.customButtonExpandAll);
            this.Controls.Add(this.customButtonCollapseAll);
            this.Controls.Add(this.customButtonEnableAll);
            this.Controls.Add(this.customButtonDisableAll);

            this.Size = new System.Drawing.Size(500, 460);
        }
    }
}

