
using System;
using System.IO;
using System.Text;
using WWFramework.WWManagers;
using WWFramework.WWXML;
using System.Collections.Generic;
using WWFramework.WWFiles;
using System.Xml;



namespace MapsPlugin
{
    public class MapManager : IMapManager
    {
        private string _rootPath;
        private ILogManager _log;
        public MapManager()
        {
        }

        public void Load(string rootPath){
            _rootPath = rootPath;
            
        }
        public void Init(){
            _log = Repository.Instance.GetManager<ILogManager>();  
            _log.WriteLn("Maps manager  initialized") ;
            
        }

        public void InitGUI(ref XmlDocument doc){
            _log.WriteLn("In maps InitGUI");
             IUIManager uIManager = Repository.Instance.GetManager<IUIManager>();
             StringBuilder xmlStr = new StringBuilder();
             xmlStr.Append(@"<AccordionSection expanded=""0"" name=""Maps"">
                <AccordionSectionHeader>
                    <AccordionSectionHeaderIcon />
                    <Text>Maps</Text>
                </AccordionSectionHeader>
                      
                <AccordionSectionContent> 
                    <VerticalScrollView>        
                        <VerticalLayout>
             ");
             DirectoryInfo di = new DirectoryInfo(_rootPath+"/maps");
             List<FileInfo> fileList = new List<FileInfo>(); 
             FileSystemUtils.VisitFileTree(di,null,null,(FileInfo fi) => {
                 if (Path.GetExtension(fi.Name)!=".meta"){
                    fileList.Add(fi);
                 }
             });
             foreach(FileInfo fi in fileList){
                 xmlStr.Append(@"<Text onClick=""SendParamEvent(MAPBUTTONEVT,");
                 xmlStr.Append(fi.FullName);
                 xmlStr.Append(@")"">");
                 xmlStr.Append(Path.GetFileNameWithoutExtension(fi.Name));
                 xmlStr.Append("</Text>\n");
             }
             _log.WriteLn(xmlStr.ToString());
             xmlStr.Append(@"
                        </VerticalLayout>  
                    </VerticalScrollView>
                </AccordionSectionContent>  
            </AccordionSection>");
            XmlManipulator manip = new XmlManipulator(doc.OuterXml);
            manip.AddStringToNodeID("AccordianX",xmlStr.ToString());
            doc.LoadXml(manip.Text);
            //_log.WriteLn(doc.OuterXml);
            uIManager.AddUIEventHandler("MAPBUTTONEVT",ButtonCB);
        }

        public void ButtonCB(string arg){
            _log.WriteLn("In ButtonCB");
            MapSelected(arg);
            
        }

        public void MapSelected(string mapPath){
            byte[] mapdata = File.ReadAllBytes(mapPath);
            Repository.Instance.GetManager<ITableTopManager>().SetMap(mapdata);
        }
    }
}
