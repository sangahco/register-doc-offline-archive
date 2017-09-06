﻿using pmis;
using pmis.i18n;
using pmis.register;
using pmis.reviewinfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pmis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SettingWindow settingForm;
        private IDbConnection daoService;
        private RegisterDocumentDataService registerDocumentDataService;
        private RegisterDocumentPresenter registerDocumentPresenter;
        private RegisterDocumentDetailView registerDocumentDetailView;
        private ReviewInfoPresenter reviewInfoPresenter;
        private ReviewInfoDataService reviewInfoDataService;
        private BindingSource fileManagerBS;
        private BindingSource reviewFilesBS;
        private BindingSource pictureFilesBS;
        private Window aboutForm;
        private PicturePresenter picturePresenter;
        private PictureViewerService pictureViewerService;

        public event EventHandler OnShowRegisterDocumentInfo;
        public event EventHandler OnShowRegisterDocumentList;

        public RegisterDocumentDetailView RegisterDocumentDetailView
        {
            get { return this.registerDocumentDetailView; }
        }

        public IDbConnection DaoService
        {
            get { return daoService; }
        }

        public string SearchCriteriaDocNumber
        {
            get { return srchNumber.Text; }
            set { srchNumber.Text = value; }
        }

        public string SearchCriteriaTitle
        {
            get { return srchTitle.Text; }
            set { srchTitle.Text = value; }
        }

        public string SearchCriteriaFromDate { get { return srchFromDate.Text; } }

        public string SearchCriteriaToDate { get { return srchToDate.Text; } }

        public string SearchCriteriaStatus
        {
            get { return srchStatus.Text; }
            set { srchStatus.Text = value; }
        }

        public string SearchCriteriaDiscipline
        {
            get { return srchDiscipline.Text; }
            set { srchDiscipline.Text = value; }
        }

        public string SearchCriteriaType
        {
            get { return srchType.Text; }
            set { srchType.Text = value; }
        }

        public string SearchCriteriaAllHistory { get { return srchHistory.Text; } }

        public string SearchCriteriaRegisteredBy { get { return srchRegisteredBy.Text; } }

        public ImageSource ImageBox
        {
            set
            {
                this.pictureBox1.Source = value;
            }
        }

        public IEnumerable DocumentList
        {
            set { registerDataGridView.ItemsSource = value; }
        }

        public IEnumerable ReviewInfoList
        {
            set { reviewDataGridView.ItemsSource = value; }
        }

        public object RegisterFilesDS
        {
            get { return fileManagerBS.DataSource; }
            set { fileManagerBS.DataSource = value; }
        }

        public object ReviewFilesDS
        {
            get { return reviewFilesBS.DataSource; }
            set { reviewFilesBS.DataSource = value; }
        }

        public object PictureFilesDS
        {
            set { pictureFilesBS.DataSource = value; }
        }

        public object PictureDirectoriesDS
        {
            set {
                pictureFolderListBox.ItemsSource = new BindingSource(value, null);
            }
        }

        public MainWindow()
        {
            // configure user folder in appdata
            try
            {
                AppConfig.InitConfig();
            }
            catch (Exception e)
            {
                new ApplicationException("Application didn't start correctly, please check the log.", e)
                    .Log()
                    .Display();
            }

            InitializeComponent();

            daoService = new SQLiteDaoService(Properties.Settings.Default.sqlite_db_location);

            try
            {
                // open db connection before doing anything else
                daoService.Open();
            }
            catch (Exception ex)
            {
                ex.Log().Display();
                return;
            }

            registerDocumentDataService = new RegisterDocumentDataService(daoService as IRegisterDocumentDao);
            registerDocumentPresenter = new RegisterDocumentPresenter(this, registerDocumentDataService);
            registerDocumentDetailView = new RegisterDocumentDetailView(this);
            registerDataGridView.CanUserAddRows = false;
            registerDataGridView.AutoGenerateColumns = false;

            reviewInfoDataService = new ReviewInfoDataService(daoService as IReviewInfoDao);
            reviewInfoPresenter = new ReviewInfoPresenter(this, reviewInfoDataService, registerDocumentDataService);
            reviewDataGridView.AutoGenerateColumns = false;
            reviewDataGridView.CanUserAddRows = false;

            reviewFilesBS = new BindingSource();
            reviewFilesBS.DataSource = new List<RegisterFile>();
            reviewFilesBS.AllowNew = false;
            reviewFileDataGrid.AutoGenerateColumns = false;
            reviewFileDataGrid.ItemsSource = reviewFilesBS;

            fileManagerBS = new BindingSource();
            fileManagerBS.DataSource = new List<RegisterFile>();
            fileManagerBS.AllowNew = false;
            fileManagerDataGridView.AutoGenerateColumns = false;
            fileManagerDataGridView.ItemsSource = fileManagerBS;

            srchHistory.ItemsSource = new BindingSource(AppConfig.RegisterHistoryOptions, null);
            srchHistory.DisplayMemberPath = "Value";
            srchHistory.SelectedValuePath = "Key";
            srchHistory.SelectedValue = AppConfig.HISTORY_LATEST;

            settingForm = new SettingWindow(
                registerDocumentDataService,
                reviewInfoDataService);

            // adding sqlite module to setting form
            settingForm.SQLiteDaoService = daoService as SQLiteDaoService;

            settingForm.SettingChanged += LoadSearchOptions;
            settingForm.SettingChanged += LoadPictureViewer;

            aboutForm = new AboutBox();

            LanguageSupport i18n = new LanguageSupport();
            i18n.SetMainFromLanguage(this);

            try
            {
                // load search options
                LoadSearchOptions();

                // request docs list
                ShowRegisterList();

                // load picture viewer
                LoadPictureViewer();
            }
            catch (Exception ex)
            {
                ex.Log().Display();
                return;
            }

        }

        private void LoadSearchOptions(object sender = null, EventArgs args = null)
        {
            string[] statuses = new string[Properties.Settings.Default.register_status.Count + 1];
            statuses[0] = "";
            Properties.Settings.Default.register_status.CopyTo(statuses, 1);
            srchStatus.ItemsSource = statuses;

            string[] disciplines = new string[Properties.Settings.Default.register_discipline.Count + 1];
            disciplines[0] = "";
            Properties.Settings.Default.register_discipline.CopyTo(disciplines, 1);
            srchDiscipline.ItemsSource = disciplines;

            string[] types = new string[Properties.Settings.Default.register_type.Count + 1];
            types[0] = "";
            Properties.Settings.Default.register_type.CopyTo(types, 1);
            srchType.ItemsSource = types;
        }

        private void LoadPictureViewer(object sender = null, EventArgs args = null)
        {
            if (pictureViewerService == null)
            {
                pictureFilesBS = new BindingSource();
                pictureFilesBS.DataSource = new List<RegisterFile>();
                pictureFilesBS.AllowNew = false;
                pictureGridView.AutoGenerateColumns = false;
                pictureGridView.ItemsSource = pictureFilesBS;

                pictureFolderListBox.Items.Clear();
                pictureFolderListBox.DisplayMemberPath = "Value";
                pictureFolderListBox.SelectedValuePath = "Key";

                pictureViewerService = new PictureViewerService();
                pictureViewerService.OnPictureSelected += SelectPictureFileOnSelection;

                picturePresenter = new PicturePresenter(this, pictureViewerService);
            }
            picturePresenter.LoadPictureDirectories();
        }

        private void ShowRegisterList()
        {
            try
            {
                if (OnShowRegisterDocumentList != null)
                    OnShowRegisterDocumentList(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void SearchButtonClickHandler(object sender, EventArgs e)
        {
            try
            {
                ShowRegisterList();
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            daoService.Close();
        }

        private void registerDataGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    System.Windows.Controls.DataGrid grid = sender as System.Windows.Controls.DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                    {
                        DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                        Console.WriteLine(dgr.Item);
                        DataRow dr = dgr.Item as DataRow;
                        this.registerDocumentDetailView.Number = Convert.ToString(dr["docno"]);
                        this.registerDocumentDetailView.Version = Convert.ToString(dr["doc_version"]);

                        if (OnShowRegisterDocumentInfo != null)
                        {
                            OnShowRegisterDocumentInfo(this, EventArgs.Empty);
                        }

                        tabControl1.SelectedIndex = 1;
                    }
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void fileManagerDataGridView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //System.Windows.Controls.DataGrid grid = sender as System.Windows.Controls.DataGrid;
                //DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;

                DependencyObject dep = (DependencyObject)e.OriginalSource;
                // iteratively traverse the visual tree
                while ((dep != null) 
                    && !(dep is System.Windows.Controls.DataGridCell))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep is System.Windows.Controls.DataGridCell)
                {
                    var cell = dep as System.Windows.Controls.DataGridCell;
                    // navigate further up the tree
                    while ((dep != null) && !(dep is DataGridRow))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }
                    DataGridRow row = dep as DataGridRow;

                    DataGridBoundColumn col = cell.Column as DataGridBoundColumn;

                    if ( col.DisplayIndex == 2 )
                    {
                        RegisterFile file = row.Item as RegisterFile;
                        RegisterFileService.OpenRegisterFileLocation(file);
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowRegisterList();
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void searchClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchCriteriaDiscipline = "";
            SearchCriteriaStatus = "";
            SearchCriteriaType = "";
            SearchCriteriaDocNumber = "";
            SearchCriteriaTitle = "";
        }

        private void settingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            settingForm.Show();
        }

        private void fileManagerDataGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;
                // iteratively traverse the visual tree
                while ((dep != null)
                    && !(dep is System.Windows.Controls.DataGridCell))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep is System.Windows.Controls.DataGridCell)
                {
                    var cell = dep as System.Windows.Controls.DataGridCell;
                    // navigate further up the tree
                    while ((dep != null) && !(dep is DataGridRow))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }
                    DataGridRow row = dep as DataGridRow;
                    RegisterFile file = row.Item as RegisterFile;
                    RegisterFileService.OpenRegisterFile(file);
                }
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void pictureNextButtonOnClick(object sender, RoutedEventArgs e)
        {
            picturePresenter.NextImage();
        }

        private void picturePreviousButtonOnClick(object sender, RoutedEventArgs e)
        {
            picturePresenter.PreviousImage();
        }

        private void pictureGridView_CellClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;
                // iteratively traverse the visual tree
                while ((dep != null)
                    && !(dep is System.Windows.Controls.DataGridCell))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep is System.Windows.Controls.DataGridCell)
                {
                    var cell = dep as System.Windows.Controls.DataGridCell;
                    // navigate further up the tree
                    while ((dep != null) && !(dep is DataGridRow))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }
                    DataGridRow row = dep as DataGridRow;
                    RegisterFile file = row.Item as RegisterFile;

                    picturePresenter.ShowImage(file);
                }
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void SelectPictureFileOnSelection(object sender, RegisterFile file)
        {
            //foreach (var row in pictureGridView.Items)
            //{
            //    var _t = row as RegisterFile;
            //    if (_t.FilePath.Equals(file.FilePath))
            //    {
            //        pictureFilesBS.Position = row.Index;
            //        break;
            //    }
            //}
        }

        private void pictureFolderListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                picturePresenter.LoadPictureFiles(pictureFolderListBox.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                ex.Log().Display();
            }
        }

        private void closeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            aboutForm.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            System.Windows.Application.Current.Shutdown();
        }
    }
}
