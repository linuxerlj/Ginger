#region License
/*
Copyright © 2014-2019 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using Amdocs.Ginger.Repository;
using Amdocs.Ginger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Ginger.Reports;
using Amdocs.Ginger;
using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Common.InterfacesLib;
using GingerCore;
using GingerCore.DataSource;

namespace Ginger.Run.RunSetActions
{
    public class RunSetActionHTMLReport : RunSetActionBase
    {
        public new static class Fields
        {
            public static string HTMLReportFolderName = "HTMLReportFolderName";
            public static string isHTMLReportFolderNameUsed = "isHTMLReportFolderNameUsed";
            public static string isHTMLReportPermanentFolderNameUsed = "isHTMLReportPermanentFolderNameUsed";
            public static string selectedHTMLReportTemplateID = "selectedHTMLReportTemplateID";
        }

        public override List<RunSetActionBase.eRunAt> GetRunOptions()
        {
            List<RunSetActionBase.eRunAt> list = new List<RunSetActionBase.eRunAt>();
            list.Add(RunSetActionBase.eRunAt.ExecutionEnd);
            return list;
        }

        public override bool SupportRunOnConfig
        {
            get { return true; }
        }

        private string mHTMLReportFolderName;
        [IsSerializedForLocalRepository]
        public string HTMLReportFolderName
        {
            get
            {
                return mHTMLReportFolderName;
            }
            set
            {
                mHTMLReportFolderName = value;
                OnPropertyChanged(nameof(HTMLReportFolderName));
            }
        }
        ValueExpression mValueExpression = null;
        ValueExpression mVE
        {
            get
            {
                if (mValueExpression == null)
                {
                    mValueExpression = new ValueExpression(WorkSpace.RunsetExecutor.RunsetExecutionEnvironment, null, WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<DataSourceBase>(), false, "", false);
                }
                return mValueExpression;
            }
        }
        [IsSerializedForLocalRepository]
        public int selectedHTMLReportTemplateID { get; set; }

        [IsSerializedForLocalRepository]
        public bool isHTMLReportFolderNameUsed { get; set; }

        [IsSerializedForLocalRepository]
        public bool isHTMLReportPermanentFolderNameUsed { get; set; }

        public override void Execute(ReportInfo RI)
        {
            string reportsResultFolder = string.Empty;
            HTMLReportsConfiguration currentConf = WorkSpace.Instance.Solution.HTMLReportsConfigurationSetList.Where(x => (x.IsSelected == true)).FirstOrDefault();
            if (WorkSpace.RunsetExecutor.RunSetConfig.RunsetExecLoggerPopulated)
            {
                string runSetFolder = string.Empty;
                if (WorkSpace.RunsetExecutor.RunSetConfig.LastRunsetLoggerFolder != null)
                { 
                    runSetFolder = WorkSpace.RunsetExecutor.RunSetConfig.LastRunsetLoggerFolder;
                    AutoLogProxy.UserOperationStart("Online Report");
                }
                else
                {
                    runSetFolder = ExecutionLogger.GetRunSetLastExecutionLogFolderOffline();
                    AutoLogProxy.UserOperationStart("Offline Report");
                }
                if (!string.IsNullOrEmpty(selectedHTMLReportTemplateID.ToString()))
                {
                    ObservableList<HTMLReportConfiguration> HTMLReportConfigurations = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<HTMLReportConfiguration>();
                    if ((isHTMLReportFolderNameUsed) && (HTMLReportFolderName != null) && (HTMLReportFolderName != string.Empty))
                    {
                        mVE.Value = HTMLReportFolderName;
                        string mHTMLReportFolderName = mVE.ValueCalculated;
                        string currentHTMLFolderName = string.Empty;
                        if (!isHTMLReportPermanentFolderNameUsed)
                        {
                            currentHTMLFolderName = mHTMLReportFolderName + "\\" + System.IO.Path.GetFileName(runSetFolder);
                        }
                        else
                        {
                            currentHTMLFolderName = mHTMLReportFolderName;
                        }
                        reportsResultFolder = Ginger.Reports.GingerExecutionReport.ExtensionMethods.CreateGingerExecutionReport(new ReportInfo(runSetFolder),
                                                                                                                                false,
                                                                                                                                HTMLReportConfigurations.Where(x => (x.ID == selectedHTMLReportTemplateID)).FirstOrDefault(),
                                                                                                                                currentHTMLFolderName,
                                                                                                                                isHTMLReportPermanentFolderNameUsed, currentConf.HTMLReportConfigurationMaximalFolderSize);
                    }
                    else
                    {
                        reportsResultFolder = Ginger.Reports.GingerExecutionReport.ExtensionMethods.CreateGingerExecutionReport(new ReportInfo(runSetFolder),
                                                                                                                                false,
                                                                                                                                HTMLReportConfigurations.Where(x => (x.ID == selectedHTMLReportTemplateID)).FirstOrDefault(),
                                                                                                                                null,
                                                                                                                                isHTMLReportPermanentFolderNameUsed);
                    }
                }
                else
                {
                    reportsResultFolder = Ginger.Reports.GingerExecutionReport.ExtensionMethods.CreateGingerExecutionReport(new ReportInfo(runSetFolder), 
                                                                                                                                false,
                                                                                                                                null,
                                                                                                                                null,
                                                                                                                                isHTMLReportPermanentFolderNameUsed);
                }
            }
            else
            {
                Errors = "In order to get HTML report, please, perform executions before";
                Reporter.HideStatusMessage();
                Status = Ginger.Run.RunSetActions.RunSetActionBase.eRunSetActionStatus.Failed;
                return;
            }
        }

        public override string GetEditPage()
        {
           // RunSetActionHTMLReportEditPage p = new RunSetActionHTMLReportEditPage(this);
            return "RunSetActionHTMLReportEditPage";
        }

        

        public override void PrepareDuringExecAction(ObservableList<GingerRunner> Gingers)
        {
            throw new NotImplementedException();
        }

        public override string Type { get { return "Produce HTML Report"; } }
    }
}
