﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeView.Model;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel.Design.Serialization;
using FungiParadise.Model;
using System.Text.RegularExpressions;

namespace FungiParadise.Gui
{
    public partial class TreeTab : UserControl
    {
        //Attributes
        private Manager manager;
        private TreeNode<CircleNode> root;

        //Constructor
        public TreeTab()
        {
            InitializeComponent();
        }

        //Initializers
        public void InitializeOrientationComboBox()
        {
            orientationComboBox.Items.AddRange(new string[] { "Vertical", "Horizontal" });
            orientationComboBox.SelectedIndex = 0;
            orientationComboBox.Enabled = true;
        }

        public void InitializeTypeComboBox()
        {
            typeComboBox.Items.AddRange(new string[] { "Accord .NET Framework", "Fungi Paradise" });
            typeComboBox.SelectedIndex = 0;
            typeComboBox.Enabled = true;
        }

        //Method
        public void InitializeTreeTab(Manager manager)
        {
            this.manager = manager;
            GenerateDecisionTreeLib();
            GenerateDecisionTreeOrg();
            InitializeOrientationComboBox();
            InitializeTypeComboBox();
            AccuracyPercentageTreeLib();
        }

        private void GenerateDecisionTreeOrg()
        {
            //Generate Tree
            manager.GenerateDecisionTreeOrg();

            //Root
            this.root = new TreeNode<CircleNode>(new CircleNode(manager.DecisionTreeOrg.RootNode.ToString()));

            //Branches
            for (int i = 0; i < manager.DecisionTreeOrg.RootNode.Children.Length; i++)
            {
                AddNode(root, manager.DecisionTreeOrg.RootNode.Questions[i], manager.DecisionTreeOrg.RootNode.Children[i]);
            }

            //AccuracyPercentage
            AccuracyPercentageTreeOrg();

            //Arrange
            VerticalOrientation();
        }

        private void GenerateDecisionTreeLib()
        {
            //Generate Tree
            manager.GenerateDecisionTreeLib();

            string[] lines = manager.getDesicionTreeLib().Split('\n');

            string root = new Regex(@"\(([^)]*)\)").Match(lines[0]).ToString();
            root = new Regex(@"\s|[().]").Replace(root, "");
            root = string.Concat(Regex.Matches(root, "[A-Z]").OfType<Match>().Select(match => match.Value));

            //Root
            this.root = new TreeNode<CircleNode>(new CircleNode(root));

            for(int i = 0; i < lines.Length; i++)
            {

            }

            //AccuracyPercentage
            AccuracyPercentageTreeLib();

            //Arrange
            VerticalOrientation();
        }

        public void AddNodeLib()
        {

        }

        public void GenerateDecisionTreeLibAux(string tree)
        {
            string[] lines = tree.Split('\n');

            Regex regex = new Regex(@"\(([^)]*)\)");
            MatchCollection matches = regex.Matches(lines[3]);
            
            for(int i = 0; i < matches.Count; i++)
            {
                Console.WriteLine(matches[i]);
            }

            for (int i = 0; i < lines.Length; i++)
            {
               //MatchCollection matches = regex.Matches(lines[i]);
            }
        }

        private void AccuracyPercentageTreeOrg()
        {
            accuracyLabel.Text = "Accuracy Percentage: " + (manager.DecisionTreeAccuracyPercentageOrg() * 100) + "%";
        }

        private void AccuracyPercentageTreeLib()
        {
            accuracyLabel.Text = "Accuracy Percentage: " + (manager.DecisionTreeAccuracyPercentageLib() * 100) + "%"; 
        }

        private void AddNode(TreeNode<CircleNode> parent, string question, DecisionTree.Model.Node child)
        {
            TreeNode<CircleNode> parentDraw = new TreeNode<CircleNode>(new CircleNode(question + ". " + child.ToString()));
            parent.AddChild(parentDraw);

            if (child is DecisionTree.Model.Decision)
            {
                DecisionTree.Model.Decision dChild = (DecisionTree.Model.Decision)child;
                for (int i = 0; i < dChild.Children.Length; i++)
                {
                    AddNode(parentDraw, dChild.Questions[i], dChild.Children[i]);
                }
            }
        }

        private void VerticalOrientation()
        {
            root.SetTreeDrawingParameters(20, 2, 20, 8, TreeNode<CircleNode>.Orientations.Vertical);
            ArrangeTree();
            picTree.Size = new Size(500, 900);
        }

        private void HorizontalOrientation()
        {
            root.SetTreeDrawingParameters(5, 80, 20, 5, TreeNode<CircleNode>.Orientations.Horizontal);
            ArrangeTree();
            picTree.Size = new Size(2080, 600);
        }

        private void ArrangeTree()
        {
            using (Graphics gr = picTree.CreateGraphics())
            {
                if (root.orientation == TreeNode<CircleNode>.Orientations.Horizontal)
                {
                    float xmin = 0, ymin = 0;
                    root.Arrange(gr, ref xmin, ref ymin);

                    xmin = (picTree.ClientSize.Width - xmin) / 2;
                    ymin = 10;
                    root.Arrange(gr, ref xmin, ref ymin);
                }
                else
                {
                    float xmin = root.indent;
                    float ymin = xmin;
                    root.Arrange(gr, ref xmin, ref ymin);
                }
            }

            picTree.Refresh();
        }

        //Trigger
        public void GenerateDecisionTree(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex == 0)
                GenerateDecisionTreeLib();
            else
                GenerateDecisionTreeOrg();

            orientationComboBox.SelectedIndex = 0;
        }

        private void PicTreePaint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            if (root != null)
                root.DrawTree(e.Graphics);
        }

        private void PicTreeReSize(object sender, EventArgs e)
        {
            if (root != null)
                ArrangeTree();
        }

        public void OrientationComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            int index = orientationComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    VerticalOrientation();
                    break;

                case 1:
                    HorizontalOrientation();
                    break;
            }
        }
    }
}
