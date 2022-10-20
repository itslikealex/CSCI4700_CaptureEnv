using CaptureManagerToCSharpProxy;
using CaptureManagerToCSharpProxy.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Xml;

namespace CSCI4700_CaptureEnv
{
    delegate void ChangeState();

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static event ChangeState ChangeState;
        public static CaptureManager CaptureManager;
        private static readonly List<ISource> _sources = new();
        public static List<ISource> SourceItems = new();
        
        private string newOutputFolder;
        int globalFramerate;
        public string OutputFolder
        {
            get { return newOutputFolder; }
            set { newOutputFolder = value; }
        }
        public int GlobalFramerate
        {
            get { return globalFramerate; }
            set { globalFramerate = value; }
        }

        public string _targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        private ISessionControl _iSessionControl;
        private ISession _iSession;
        private ISinkControl _sinkControl;
        private ISourceControl _sourceControl;
        private IEncoderControl _encoderControl;
        private IStreamControl _streamControl;
        private ISpreaderNodeFactory _spreaderNodeFactory;

        private bool _mIsStarted;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            ChangeState += MainWindow_mChangeState;
        }

        void MainWindow_mChangeState()
        {
            m_StartStopBtn.IsEnabled = _sources.Count != 0;
        }

        private void MainWindow_WriteDelegateEvent(string aMessage)
        {
            MessageBox.Show(aMessage);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CaptureManager = new CaptureManager("CaptureManager.dll");
            }
            catch (Exception)
            {
                try
                {
                    CaptureManager = new CaptureManager();
                }
                catch (Exception)
                {

                }
            }

            LogManager.getInstance().WriteDelegateEvent += MainWindow_WriteDelegateEvent;

            if (CaptureManager is null)
            {
                return;
            }

            _sourceControl = CaptureManager.createSourceControl();

            if (_sourceControl is null)
            {
                return;
            }

            _encoderControl = CaptureManager.createEncoderControl();

            if (_encoderControl is null)
            {
                return;
            }

            _sinkControl = CaptureManager.createSinkControl();

            if (_sinkControl is null)
            {
                return;
            }

            _iSessionControl = CaptureManager.createSessionControl();

            if (_iSessionControl is null)
            {
                return;
            }

            _streamControl = CaptureManager.createStreamControl();

            if (_streamControl is null)
            {
                return;
            }

            _streamControl.createStreamControlNodeFactory(ref _spreaderNodeFactory);

            if (_spreaderNodeFactory is null)
            {
                return;
            }

            XmlDataProvider lXmlDataProvider = (XmlDataProvider)Resources["XmlSources"];
            if (lXmlDataProvider is null)
            {
                return;
            }

            XmlDocument doc = new();
            string lxmldoc = "";
            CaptureManager.getCollectionOfSources(ref lxmldoc);
            doc.LoadXml(lxmldoc);
            lXmlDataProvider.Document = doc;
        }

        public static void addSourceControl(ISource aISource)
        {
            _sources.Add(aISource);

            ChangeState?.Invoke();
        }

        public static void removeSourceControl(ISource aISource)
        {
            _sources.Remove(aISource);

            ChangeState?.Invoke();
        }

        private void mControlBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_mIsStarted)
            {
                _mIsStarted = false;

                if (_iSession is null)
                {
                    return;
                }

                _iSession.stopSession();

                _iSession.closeSession();

                _iSession = null;

                m_StartStopBtn.Content = "Start";

                foreach (var item in SourceItems)
                {
                    item?.Access(true);
                }

                return;
            }
            else
            {
                //string filename = DateTime.Now.ToString("MM/dd/yyyy_HH:mm:ss").ToString();
                List<object> lOutputNodes = new();

                foreach (ISource source in _sources)
                {
                    List<object> lCompressedMediaTypeList = new();
                    var lCompressedMediaType = source.GetCompressedMediaType();
                    if (lCompressedMediaType != null)
                    {
                        lCompressedMediaTypeList.Add(lCompressedMediaType);
                    }

                    _sinkControl.createSinkFactory(
                        Guid.Parse("A2A56DA1-EB84-460E-9F05-FEE51D8C81E3"),
                        out IFileSinkFactory lFileSinkFactory);
                    lOutputNodes.AddRange(
                        getOutputNodes(lCompressedMediaTypeList, lFileSinkFactory, $"{source.FriendlyName}.asf"));
                }

                if (lOutputNodes is null || lOutputNodes.Count == 0)
                {
                    return;
                }

                List<object> lSourceNodes = new();
                for (int i = 0; i < lOutputNodes.Count; i++)
                {
                    var lSourceNode = _sources[i].GetSourceNode(lOutputNodes[i]);
                    if (lSourceNode != null)
                    {
                        lSourceNodes.Add(lSourceNode);
                    }
                }

                _iSession = _iSessionControl.createSession(lSourceNodes.ToArray());
                if (_iSession is null)
                {
                    return;
                }

                if (_iSession.startSession(0, Guid.Empty))
                {
                    m_StartStopBtn.Content = "Stop";
                }

                _mIsStarted = true;

                foreach (var item in SourceItems)
                {
                    item?.Access(false);
                }
            }
        }

        private List<object> getOutputNodes(List<object> aCompressedMediaTypeList, IFileSinkFactory aFileSinkFactory, string filename)
        {
            if (aCompressedMediaTypeList is not null &&
                aCompressedMediaTypeList.Any() &&
                aFileSinkFactory is not null)
            {

                aFileSinkFactory.createOutputNodes(
                    aCompressedMediaTypeList,
                    Path.Combine(_targetFolder, filename),
                    out List<object> lresult);
                return lresult;
            }
            else
            {
                return new List<object>();
            }
        }

        private void fileOptions_Click(object sender, RoutedEventArgs e)
        {
            var optionsPage = new Options();
            optionsPage.Owner = this;
            optionsPage.DataContext = this;
            optionsPage.Show();
        }
    }
}
