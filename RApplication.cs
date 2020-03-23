//All rights reserved Jake Gustafson 2007
//Created 2007-10-02 in Kate

using System;
using System.Drawing;//Bitmap etc
using System.Windows.Forms;//Form etc

namespace ExpertMultimedia {
	/// <summary>
	/// An interface abstractor class.  Allows for a position-free abstract interface that can then be read and translated using any outside skinning mechanism possible.
	/// </summary>
	public class RApplication {//formerly IAbstractor
		private static int iMode=0; //reference to Modes array location in calling program
		private static int iWorkspace=0;
		private static int iTool=0;
		private static int iActiveOption=0;//only for keyboard/mouseover focus
		private static int iActiveTab=0;
		private static RFont rfontDefault {
			get { return RFont.rfontDefault;}
			set { RFont.rfontDefault=value; }
		}
		public static int ActiveTabIndex {
			get { return iActiveTab; }
		}
		///<summary>
		///Turns statusbar on and off
		///</summary>
		private static int iPreviousTab=0;//TODO: set this when switching to a new or other tab
		private static RKeyboard keyboard=new RKeyboard();
		private static uint dwMouseButtons=0;
		public static uint dwButtons=0; ///TODO: finish this--the virtual/mapped gamepad buttons that are currently pressed.
		//public InteractionQ iactionq=null; //a queue of objects which each contain a cKey, iButton, or joystick op.
							//may want to split iactionq into: charq and dwButtons;
							//weigh performance vs. possible unwanted skipping of inputs.

		#region Render (framework Graphics) vars
 		public static Bitmap bmpOffscreen=null;
 		public static Graphics gOffscreen=null;
 		public static RImage riOffscreen=null;//TODO: allow setting this to the real screen buffer if using buffer transfer (non-framework-window) mode!
 		public static Graphics gTarget=null;
// 		//public System.Windows.Forms.Border3DStyle b3dstyleNow=System.Windows.Forms.Border3DStyle.Flat;
// 		//public IPoly polySelection=new IPoly();
// 		public static Color colorBack=SystemColors.Window;
// 		public static Color colorActive=RConvert.RgbRatioToColor(.7,.6,0.0);
// 		public SolidBrush brushBack=new SolidBrush(SystemColors.Window);
// 		public System.Drawing.Pen penBack=new System.Drawing.Pen(SystemColors.Window);
// 		public static Color colorTextNow=Color.Black;
// 		public static SolidBrush brushTextNow=new SolidBrush(Color.Black);
		//public static System.Drawing.Pen penTextNow=new System.Drawing.Pen(Color.Black);
		//public static System.Drawing.Font fontMonospaced=new Font("Andale Mono",9);//default monospaced font
		//FontFamily fontfamilyMonospaced = new FontFamily("Andale Mono");
		#endregion Render (framework Graphics) vars


		#region framework mode vars (when fastpanelTarget!=null)
		public static FastPanel fastpanelTarget=null;
		private static System.Windows.Forms.Keys KeyEventArgs_KeyCodeWas=(System.Windows.Forms.Keys)0;
		private static string KeyEventArgs_KeyCodeStringWas="";
		private static string KeyEventArgs_KeyCodeStringLowerWas="";
		private static bool bShift=false;
		private static bool bLastKeyWasTypable=false;
		private static bool bLastKeyIsNumber=false;
		public static MainForm mainformNow=null;
		//TODO: bCapitalize {get {return bShift!=bCapsLock; } }
		#endregion framework mode vars (when fastpanelTarget!=null)
		private static RForms[] tabs=null;
		public static int Maximum {
			get { return tabs!=null?tabs.Length:0; }
		}
		private static int iWidth=32;
		private static int iHeight=32;
		public static int Width {
			get { return iWidth; }
		}
		public static int Height {
			get { return iHeight; }
		}
		private static string sStatus="";
		///<summary>
		///Sets the statusbar text
		///Returns false if bStatusBar=false.
		/// -Set RApplication.bStatusBar=true to turn the status bar on before using this.
		///</summary>
		public static bool SetStatus(string sMsg) {
			sStatus=sMsg;
			if (fastpanelTarget!=null) InvalidatePanelIfExists();
			return bStatusBar;
		}
		public static void InvalidatePanelIfExists() {
			if (fastpanelTarget!=null) fastpanelTarget.Invalidate();
		}
		///<summary>
		///Returns the status bar text.
		///</summary>
		public static string GetStatus() {
			return sStatus;
		}
		public static bool bStatusBar=true;
		public static Percent percStatusBarHeight=new Percent();
		public static int iStatusBarHeight=18;//TODO: adjust based on percStatusBarHeight&&bStatusBar
		public static bool bTabArea=false;
		public static Percent percTabAreaHeight=new Percent();
		public static int iTabsAreaHeight=0;//TODO: adjust based on percTabHeight&&bTabArea
		///<summary>
		///Area for the actual graphics of the tab
		///</summary>
		public static int ClientX {
			get { return 0; }
		}
		public static int ClientY {
			get { return iTabsAreaHeight; }
		}
		public static int ClientWidth {
			get { return Width; }
		}
		public static int ClientHeight {
			get { return Height-iTabsAreaHeight-(bStatusBar?iStatusBarHeight:0); }
		}
		private static RForms ActiveTab {
			get { return (tabs!=null&&iActiveTab>=0&&iActiveTab<tabs.Length)
						? tabs[iActiveTab] : null ; }
			set {
				if (tabs!=null&&iActiveTab<tabs.Length) tabs[iActiveTab]=value;
				else RReporting.ShowErr("Tab out of range","creating tab","set ActiveTab value");
			}
		}
		private static StringQ sqEvents=new StringQ();
		public static int EventCount() {
			return sqEvents!=null?sqEvents.Count:0;
		}
		public static string PeekEvent(int index) {
			if (sqEvents!=null) sqEvents.Peek(index);
			return null;
		}
		public static bool AddEvent(string sEvent) {
			bool bGood=false;
			if (sqEvents!=null) bGood=sqEvents.Enq(sEvent);
			return bGood;
		}
		public static bool HasEvents() {
			return sqEvents!=null&&!sqEvents.IsEmpty;
		}
		private static bool SetHtml(string sData) {
			bool bGood=false;
			if (ActiveTab!=null) {
			}
			return bGood;
		}
		private static string[] sarrNameTemp=new string[40*2];
		private static string[] sarrValueTemp=new string[40*2];
		public static bool DoEvent(string sEvent) {
			bool bHandled=true;//changed to false below if command not native to this class
			int iParams=0;
			if (RString.IsNotBlank(sEvent)) {
				string sToLower=sEvent.ToLower();
				int iOpener=sEvent.IndexOf("(");
				int iCloser=sEvent.LastIndexOf(")");
				if (iOpener>-1&&iCloser>iOpener) {
					iParams=RString.SplitParams(ref sarrNameTemp,ref sarrValueTemp,sEvent,'=',',',iOpener+1,iCloser);
				}
				if (sToLower.StartsWith("self.close(")) {
					CloseTab(ActiveTabIndex);
				}
				else bHandled=false;
			}
			return bHandled;
		}//end DoEvent
		private static void FixPreviousTab() {
			if (iPreviousTab>=tabs.Length) iPreviousTab=tabs.Length-1;
			while (iPreviousTab>=0&&tabs[iPreviousTab]==null) iPreviousTab--;
			if (iPreviousTab<0) iPreviousTab=0;
		}
		public static bool CloseTab(int iTabX) {
			bool bGood=false;
			if (tabs!=null) {
				if (iTabX>0) {
					tabs[iTabX]=null;//TODO: ask if the page should be saved if the tab is in edit mode
					if (iActiveTab==iTabX) {
						FixPreviousTab();
						iActiveTab=iPreviousTab;
					}
				}
				else if (iTabX==0) RReporting.Warning("Cannot close the root tab","","CloseTab("+iTabX.ToString()+")");
				else RReporting.Warning("Tried to close out-of-range tab","","CloseTab("+iTabX.ToString()+")");
			}
			else RReporting.Warning("Tried to close null tab","","CloseTab("+iTabX.ToString()+")");
			return bGood;
		}
		public static bool DoEvents() {//HandleNativeMethods
			bool bGood=true;
			if (ActiveTab!=null) ActiveTab.HandleScriptableMethodsOrPassToRApplication();//TODO: do scripts in all other tabs??
			while (!sqEvents.IsEmpty) {
				string sEventNow=sqEvents.Deq();
				if (RString.IsNotBlank(sEventNow)) {
					if (ActiveTab==null||!ActiveTab.DoEvent(sEventNow)) {
						if (!DoEvent(sEventNow)) {
							RReporting.Debug("Ignored event:"+sEventNow+" (of you need to catch this event it must be done before RApplication.DoEvents is called)");
						}
					}
				}
			}
			return bGood;
		}//end DoEvents
		public static bool SetEventAsHandled(int iEvent) {
			if (sqEvents!=null) return sqEvents.Poke(iEvent,null);
			return false;
		}
		private static RForm DefaultNode {
			get { return ActiveTab!=null?ActiveTab.DefaultNode:null; }
			set { if (ActiveTab!=null) ActiveTab.DefaultNode=value; }
		}
		private static RForm ActiveNode {
			get { return ActiveTab!=null?ActiveTab.ActiveNode:null; }
			//set { if (ActiveTab!=null) ActiveTab.ActiveNode=value; }
		}
		private static RForm RootNode {
			get { return ActiveTab!=null?ActiveTab.RootNode:null; }
			set { if (ActiveTab!=null) ActiveTab.RootNode=value; }
		}
		private static RApplicationNode nodeLastCreated=null;
		public static RApplicationNode LastCreatedNode { get {return nodeLastCreated;} }
		private static string ShiftMessage() {
			return bShift?"[shift]+":"";
		}
		public static int Mode {
			get {
				return modes.Element(iMode).Value; 
			}
		}
		public static int Workspace {
			get { return iWorkspace; }
		}

		#region constructors
		public RApplication() {
			MessageBox.Show("The program should not have instanciated an RApplication object (RApplication should only be used statically)");//Init();
		}
		static RApplication() {//formerly public bool Init()
			iMode=0;
			workspaces=new RApplicationNodeStack();
			modes=new RApplicationNodeStack();
			tools=new RApplicationNodeStack();
			options=new RApplicationNodeStack();
		}
		///<summary>
		///Initializes the RApplication in sdl mode
		///</summary>
		public static void Init(int iSetWidth, int iSetHeight, string sHtml) {
			try {
				if (iSetWidth<1||iSetHeight<1) {
					RReporting.ShowErr("Window dimensions are needed for RApplication Init","checking window size","RApplication Init(iSetWidth:"+iSetWidth+", iSetHeight:"+iSetHeight+")");
					iSetWidth=800;
					iSetHeight=600;
				}
				iWidth=iSetWidth;
				iHeight=iSetHeight;
				tabs=new RForms[3];
				iActiveTab=0;
				ActiveTab=new RForms();
				if (sHtml!=null) ActiveTab.SetHtml(sHtml);
				ActiveTab.LoadMonochromeFont();
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn);
			}
		}
		///<summary>
		///Initializes the RApplication in Framework mode
		///-The program using RApplication should add these settings to the FastPanel first:
		///	panelDestX.SetStyle(ControlStyles.DoubleBuffer, true);
		///	panelDestX.SetStyle(ControlStyles.AllPaintingInWmPaint , true);
		///	panelDestX.SetStyle(ControlStyles.UserPaint, true);			
		///-The program using RApplication must also set FrameworkPanelBufferOnPaint as the 
		///OnPaint event handler for the panelDestX
		///</summary
		public static void Init(FastPanel panelDestX, string sHtml) {
			try {
				RApplication.fastpanelTarget=panelDestX;
				Init(panelDestX.Width, panelDestX.Height, sHtml);
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"accessing panel during RApplication init");
			}
		}
		public static void Init(FastPanel panelDestX) {
			Init(panelDestX,null);
		}
		#endregion constructors

		public static void Push(RForm rformAdd) {
			if (ActiveTab!=null) ActiveTab.Push(rformAdd);
		}
		public static int GetNewTabIndex() {
			for (int iNow=0; iNow<Maximum; iNow++) {
				if (tabs[iNow]==null) return iNow;
			}
			return -1;
		}
		private static int iLastCreatedTabIndex=0;
		public static int LastCreatedTabIndex {
			get { return iLastCreatedTabIndex; }
		}
		public static RForms LastCreatedTab {
			get { return (tabs!=null&&iLastCreatedTabIndex>=0&&iLastCreatedTabIndex<tabs.Length) ? tabs[iLastCreatedTabIndex] : null; }
			set { if (tabs!=null&&iLastCreatedTabIndex>=0&&iLastCreatedTabIndex<tabs.Length) tabs[iLastCreatedTabIndex]=value; }
		}
		///<summary>
		///Generates a form based on the c-style variable declarations in sarrCDecl.
		/// sarrButtons is an array of buttons that define what the callback in the
		/// RApplication event queue will look like.  If sFunctionToEnqueue=="login"
		/// and sarrButtons=={"OK","Cancel"} then the resulting fuction added to the
		/// RApplication event queue will start with either login.OK(...) or login.Cancel(...)
		/// where "..." is the parameter list is a list of assignments that corresponds
		/// to sarrCDecl. For example, the function could be
		/// login.OK(user=myuser,password=passwordx)
		/// (the ".OK", variable name, and equal sign are included by the native 
		/// RApplication form method, which is specified in the form generated by this method)
		///sarrCDecl can be simply a variable name if you want a one-line edit box displayed 
		/// to the user--to change the type and display method, use a C Delaration, i.e. 
		/// string var={"a","b"} to select a single value from drop-down list, or
		/// string[] var={"a","b"} to select multiple from a list.
		///s2dParamHtmlTagPropertyAssignments is an optional 2D array where the first dimension
		/// corresponds to sarrCDecl, describing additional html tag properties for the html form
		/// element that will be generated for each index of sarrCDecl.
		///s2dParamStyleAttribAssignments is an optional 2D array where the first dimension
		/// corresponds to sarrCDecl, describing additional style attributes for the html form
		/// element that will be generated for each index of sarrCDecl.
		///</summary>
		public static void GenerateForm(string sTitle, string sFunctionToEnqueue, string[] sarrButtons, string[] sarrCDecl, string[][] s2dParamHtmlTagPropertyAssignments, string[][] s2dParamStyleAttribAssignments) {
			int iNewTab=GetNewTabIndex();
			if (iNewTab>=0) {
				iLastCreatedTabIndex=iNewTab;
				LastCreatedTab=new RForms();
				iActiveTab=LastCreatedTabIndex;
				string sForm=LastCreatedTab.GenerateForm(sTitle, sFunctionToEnqueue, sarrButtons, sarrCDecl,s2dParamHtmlTagPropertyAssignments,s2dParamStyleAttribAssignments);
				RString.StringToFile("1.Debug GenerateForms.html",sForm); ///debug only
				int iTestCount=ActiveTab.SetHtml(sForm);
			}
			else RReporting.ShowErr("Could not allocate new tab for input form");
		}//end GenerateForm primary overload
		///<summary>
		/// sarrCDecl can be simply a variable name if you want a one-line edit box displayed 
		/// to the user--to change the type and display method, use a C Delaration, i.e. 
		/// string var={"a","b"} to select a single value from drop-down list, or
		/// string[] var={"a","b"} to select multiple from a list.
		/// For a default you can use:
		/// string[] var={"a","b","c"}=b
		/// or for multiple defaults you can use:
		/// string[] var={"a","b","c"}=a,b
		/// For default on regular text input or checkbox you can respectively use:
		/// string name="myname" or bool remember=true
		/// After the value (which is not required, you can put extra html tag properties 
		/// i.e. the disabled property in a '<' and '>' enclosure, and extra style assignments
		/// in a '{' and '}' enclosure.
		/// 
		///</summary>
		public static void GenerateForm(string sTitle, string sFunctionToEnqueue, string[] sarrButtons, string[] sarrCDecl) {
			//splits the "<>" and "{}" enclosed assignments and calls the primary overload.
			string[][] s2dPropAssignments=null;
			string[][] s2dStyleAttribAssignments=null;
			if (RReporting.AnyNotBlank(sarrCDecl)) {
				s2dPropAssignments=new string[sarrCDecl.Length][];
				s2dStyleAttribAssignments=new string[sarrCDecl.Length][];
				for (int iNow=0; iNow<sarrCDecl.Length; iNow++) {
					if (RReporting.IsNotBlank(sarrCDecl[iNow])) {
						string sToLower=sarrCDecl[iNow].ToLower();
						int iSpace=sarrCDecl[iNow].IndexOf(" ");
						int iPropertiesOpener=sarrCDecl[iNow].IndexOf("<");
						int iPropertiesCloser=sarrCDecl[iNow].IndexOf(">");
						int iStyleAttribsOpener=sarrCDecl[iNow].IndexOf("{");
						int iStyleAttribsCloser=sarrCDecl[iNow].IndexOf("}");
						int iMinCut=iPropertiesOpener<iStyleAttribsOpener?iPropertiesOpener:iStyleAttribsOpener;
						if (iPropertiesOpener>0&&iPropertiesCloser>iPropertiesOpener) {
							s2dPropAssignments[iNow]=RString.SplitScopes(sarrCDecl[iNow].Substring(iPropertiesOpener,iPropertiesCloser-iPropertiesOpener),' '); //RString.SplitAssignmentsSgml(out sarrPropName, out sarrPropVal, sarrCDecl[iNow]);
						}
						else s2dPropAssignments[iNow]=null;
						if (iStyleAttribsOpener>0&&iStyleAttribsCloser>iStyleAttribsOpener) {
							s2dStyleAttribAssignments[iNow]=RString.SplitScopes(sarrCDecl[iNow].Substring(iStyleAttribsOpener,iStyleAttribsCloser-iStyleAttribsOpener),';'); //RString.SplitAssignmentsSgml(out sarrPropName, out sarrPropVal, sarrCDecl[iNow]);
						}
						else s2dStyleAttribAssignments[iNow]=null;
						if (iMinCut>-1) {
							sarrCDecl[iNow]=RString.RemoveEndsWhiteSpace( RString.SafeSubstring(sarrCDecl[iNow],0,iMinCut) );
							//now it is a CSharp Declaration
						}
					}//end if param not blank
				}//end for params
				GenerateForm(sTitle, sFunctionToEnqueue, sarrButtons, sarrCDecl, s2dPropAssignments, s2dStyleAttribAssignments);
			}//end if any sarrCDecl not blank
		}//end GenerateForm
		public static int LastCreatedNodeIndex {
			get { return ActiveNode!=null?ActiveTab.LastCreatedNodeIndex:-1; }
		}
		public static void SetDefaultNode(int iNodeInThisTab) {
			if (ActiveTab!=null) ActiveTab.SetDefaultNode(iNodeInThisTab);
		}
		public static void SetDefaultNode(string sName) {
			if (ActiveTab!=null) ActiveTab.SetDefaultNode(sName);
		}
		public static void Refresh() {
			if (ActiveTab!=null) {//&&bFormActive) {
				fastpanelTarget.Invalidate();//causes OnPaint event
			}
		}

		#region framework mode drawing
		///<summary>
		///The program using RApplication must set this as the OnPaint event handler for the FastPanel
		///</summary>
		public static void FrameworkPanelBufferOnPaint(object sender, PaintEventArgs e) {
			
			if (ActiveTab!=null) {
				Render(e.Graphics, fastpanelTarget.Left, fastpanelTarget.Top, fastpanelTarget.Width, fastpanelTarget.Height);
			}
			//iLimiter++;
		}
		public static bool bWarnOnNextNoActiveTab=true;
		///<summary>
		///Primary Renderer
		///</summary>
		private static bool Render(Graphics gDest, int TargetLeft, int TargetTop, int TargetWidth, int TargetHeight) {
			//TODO: combine this with primary renderer by using riDest and gDest and checking which is null (overloads pass null for nonpresent parameter)
			try {
				bool bCreate=false;
				if (riOffscreen==null||riOffscreen.Width!=TargetWidth||riOffscreen.Height!=TargetHeight) bCreate=true;
				if (bmpOffscreen==null||bmpOffscreen.Width!=TargetWidth||bmpOffscreen.Height!=TargetHeight) bCreate=true;
				if (gOffscreen==null) bCreate=true;
				//if (gTarget==null) bCreate=true;
				if (bCreate) {
					riOffscreen=new RImage(TargetWidth,TargetHeight);
					bmpOffscreen=new Bitmap(TargetWidth,TargetHeight);
					gOffscreen=Graphics.FromImage(bmpOffscreen);
					gTarget=gDest;//panelAccessedForDimensionsOnly.CreateGraphics();
					RootNode.rectAbs.X=TargetLeft;
					RootNode.rectAbs.Y=TargetTop;
					RootNode.rectAbs.Width=TargetWidth;
					RootNode.rectAbs.Height=TargetHeight;
					Console.WriteLine("Created backbuffer");
				}	
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"getting backbuffer ready for Form");
			}
			bool bGood=false;
			try {
				int iDrawn=0;
				int iNow=0;
				bGood=false;
				if (ActiveTab!=null) {
					bGood=ActiveTab.Render(riOffscreen,ClientX,ClientY,ClientWidth,ClientHeight);
					rfontDefault.Render(ref riOffscreen, 3, ClientY+ClientHeight+3, sStatus);
					//TODO: debug performance -- drawing to bmp then to graphics
					riOffscreen.DrawTo(bmpOffscreen);//riOffscreen.DrawTo(gOffscreen);
					gDest.DrawImage(bmpOffscreen,0,0);
				}
				else {
					if (bWarnOnNextNoActiveTab) {
						RReporting.Warning("No active tab to render");
						bWarnOnNextNoActiveTab=false;
					}
					bGood=false;
				}
				riOffscreen.DrawTo(bmpOffscreen);//, gOffscreen);//riOffscreen.DrawTo(gOffscreen);
				gDest.DrawImage(bmpOffscreen,0,0);//debug performance*/
			}
			catch (Exception exn) {
				bGood=false;
				RReporting.ShowExn(exn,"rendering RApplication nodes to Graphics object","RForms Render {nodes:"+(ActiveTab!=null?ActiveTab.Count.ToString():"(nulltab)")+"; Graphics:"+((gDest==null)?"null":"non-null")+"; bmpOffscreen:"+((bmpOffscreen==null)?"null":"non-null")+"; riOffscreen:"+((riOffscreen==null)?"null":"non-null")+";}");
				//RReporting.ShowExn(exn,"rendering RApplication nodes to Graphics object","RForms Render {nodes:"+(ActiveTab!=null?ActiveTab.Count.ToString():"(nulltab)")+"; Graphics:"+((gDest==null)?"null":"non-null")+"; bmpOffscreen:"+((bmpOffscreen==null)?"null":"non-null")+"; riOffscreen:"+((riOffscreen==null)?"null":"non-null")+";}");
			}
			return bGood;
		}//end Render(Graphics, left, top, width, height)
		/*
		///<summary>
		///This method is for taking screenshots or possibly other uses
		///</summary>
		public static bool Render(Bitmap bmpDest) {
			bool bGood=false;
			try {
				if (bmpDest!=null) {
					System.Drawing.Graphics gDest = Graphics.FromImage(bmpDest);
					bGood=Render(gDest);
					gDest.Dispose();
				}
			}
			catch (Exception exn) {
				bGood=false;
				RReporting.ShowExn(exn,"rendering RForms to Bitmap","Render(bmpDest:non-null)");
			}
			return bGood;
		}//end Render(Bitmap) */
/*		private static bool Render(FastPanel panelDest) {
			bool bGood=false;
			bool bCreate=false;
			try {
				if (riOffscreen==null||riOffscreen.Width!=panelDest.Width||riOffscreen.Height!=panelDest.Height) bCreate=true;
				if (bmpOffscreen==null||bmpOffscreen.Width!=panelDest.Width||bmpOffscreen.Height!=panelDest.Height) bCreate=true;
				if (gOffscreen==null) bCreate=true;
				//if (gTarget==null) bCreate=true;
				if (bCreate) {
					riOffscreen=new RImage(panelDest.Width,panelDest.Height);
					bmpOffscreen=new Bitmap(panelDest.Width,panelDest.Height);
					gOffscreen=Graphics.FromImage(bmpOffscreen);
					gTarget=panelDest.CreateGraphics();
					RootNode.rectAbs.X=panelDest.Left;
					RootNode.rectAbs.Y=panelDest.Top;
					RootNode.rectAbs.Width=panelDest.Width;
					RootNode.rectAbs.Height=panelDest.Height;
				}
				bGood=Render(gTarget);
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"accessing window panel while rendering nodes","RForms Render("+RReporting.DebugStyle("panel",panelDest,false,false)+")");
			}
			return bGood;
		}//end Render(FastPanel)*/
		#endregion framework mode drawing


		#region framework mouse input
		public static void FrameworkMouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (ActiveTab!=null) {
				if (e.Button==MouseButtons.Left) ActiveTab.RMouseUpdate(true,1);
				else if (e.Button==MouseButtons.Right) ActiveTab.RMouseUpdate(true,2);
				InvalidatePanelIfExists();
			}
			if (mainformNow!=null) mainformNow.HandleScriptableMethods();
			else RReporting.Debug("FrameworkMouseDown: RApplication.mainformNow is null");
		}
		public static void FrameworkMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (ActiveTab!=null) {
				if (e.Button==MouseButtons.Left) ActiveTab.RMouseUpdate(false,1);
				else if (e.Button==MouseButtons.Right) ActiveTab.RMouseUpdate(false,2);
				InvalidatePanelIfExists();
			}
			if (mainformNow!=null) mainformNow.HandleScriptableMethods();
			else RReporting.Debug("FrameworkMouseUp: RApplication.mainformNow is null");
		}
		public static void FrameworkMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (ActiveTab!=null) {
				ActiveTab.RMouseUpdate(e.X, e.Y,fastpanelTarget.Left,fastpanelTarget.Top);
				InvalidatePanelIfExists();
			}
			if (mainformNow!=null) mainformNow.HandleScriptableMethods();
			else RReporting.Debug("FrameworkMouseMove: RApplication.mainformNow is null");
		}
		#endregion framework mouse input

		#region framework keyboard input
		public static void FrameworkKeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			string KeyUp_KeyEventArgs_KeyCodeStringWas=e.KeyCode.ToString();
			string KeyUp_KeyEventArgs_KeyCodeStringLowerWas=KeyUp_KeyEventArgs_KeyCodeStringWas.ToLower();
			if (KeyUp_KeyEventArgs_KeyCodeStringLowerWas=="shiftkey") { //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
				bShift=false;
			}
		}
		public static void FrameworkKeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			bool bGood=false;
			RForm InputTargetNow=null;
			if (ActiveNode!=null&&ActiveNode.Visible==true) InputTargetNow=ActiveNode;//if (ActiveTab!=null&&ActiveTab.ActiveNode!=null&&ActiveTab.ActiveNode.Visible==true) InputTargetNow=ActiveNode;
			else InputTargetNow=DefaultNode;
			if (InputTargetNow!=null) {
				bLastKeyIsNumber=true;
				bLastKeyWasTypable=false;
				if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) {
					if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9) {
						bLastKeyIsNumber=false;
					}
				}
				//if () bLastKeyIsTyping=true;
				//else bLastKeyIsTyping=false;
				KeyEventArgs_KeyCodeWas=e.KeyCode;
				KeyEventArgs_KeyCodeStringWas=e.KeyCode.ToString();
				KeyEventArgs_KeyCodeStringLowerWas=KeyEventArgs_KeyCodeStringWas.ToLower();
				if (fastpanelTarget!=null) fastpanelTarget.Focus();
				string sTest="";
				if (!bLastKeyIsNumber) {
					//e.Handled = true; //Stop the character from being entered into the control if it is non-numerical.
				}
				sTest=ShiftMessage()+KeyEventArgs_KeyCodeStringWas;
				
				if (KeyEventArgs_KeyCodeStringLowerWas.Length>1) {
					if (KeyEventArgs_KeyCodeStringLowerWas=="back")  bGood=InputTargetNow.Backspace(); //e.KeyChar=='\b') {
					else if (e.KeyCode==Keys.Return||KeyEventArgs_KeyCodeStringLowerWas=="enter") bGood=InputTargetNow.Return(); //e.KeyChar==(char)Keys.Return) {//else if (KeyEventArgs_KeyCodeWas==Keys.Return) {
					else if (KeyEventArgs_KeyCodeStringLowerWas=="delete") bGood=InputTargetNow.Delete(); //KeyEventArgs_KeyCodeWas==Keys.Delete) {
					else if (KeyEventArgs_KeyCodeStringLowerWas=="up") bGood=InputTargetNow.ShiftSelection(-1,0,bShift); //KeyEventArgs_KeyCodeWas==Keys.Up) {//up
					else if (KeyEventArgs_KeyCodeStringLowerWas=="down") bGood=InputTargetNow.ShiftSelection(1,0,bShift); //KeyEventArgs_KeyCodeWas==Keys.Down) {//down
					else if (KeyEventArgs_KeyCodeStringLowerWas=="left") bGood=InputTargetNow.ShiftSelection(0,-1,bShift); //KeyEventArgs_KeyCodeWas==Keys.Left) {//left
					else if (KeyEventArgs_KeyCodeStringLowerWas=="right") bGood=InputTargetNow.ShiftSelection(0,1,bShift); //KeyEventArgs_KeyCodeWas==Keys.Right) {//right
					else if (KeyEventArgs_KeyCodeStringLowerWas=="tab") { //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
						//TODO: bGood=InputTargetNow.Tab();//InputTargetNow.Insert("\t");//commented for debug only
					}
					else if (KeyEventArgs_KeyCodeStringLowerWas=="space") bGood=InputTargetNow.Insert(" "); //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					else if (KeyEventArgs_KeyCodeStringLowerWas=="home") bGood=InputTargetNow.Home(bShift);
					else if (KeyEventArgs_KeyCodeStringLowerWas=="end") bGood=InputTargetNow.End(bShift);
					else if (KeyEventArgs_KeyCodeStringLowerWas.StartsWith("oem")) bLastKeyWasTypable=true;
					else if (KeyEventArgs_KeyCodeStringLowerWas.Length==2&&KeyEventArgs_KeyCodeStringLowerWas.StartsWith("d") ) bLastKeyWasTypable=true; //i.e. "d0" (top row zero) 
					//else if (KeyEventArgs_KeyCodeStringLowerWas=="oemminus") bGood=InputTargetNow.Insert("-"); //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					//else if (KeyEventArgs_KeyCodeStringLowerWas=="oemplus") bGood=InputTargetNow.Insert("+"); //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					else if (KeyEventArgs_KeyCodeStringLowerWas.StartsWith("numpad")) bGood=InputTargetNow.Insert(KeyEventArgs_KeyCodeStringLowerWas.Substring(6)); //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					else if (KeyEventArgs_KeyCodeStringLowerWas=="add") bGood=InputTargetNow.Insert("+"); //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					else if (KeyEventArgs_KeyCodeStringLowerWas=="subtract") bGood=InputTargetNow.Insert("-"); //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					else if (KeyEventArgs_KeyCodeStringLowerWas=="multiply") bGood=InputTargetNow.Insert("*"); //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					else if (KeyEventArgs_KeyCodeStringLowerWas=="divide") bGood=InputTargetNow.Insert("/"); //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					else if (KeyEventArgs_KeyCodeStringLowerWas=="shiftkey") bShift=true; //e.KeyChar=='\t') {//else if (KeyEventArgs_KeyCodeWas==Keys.Tab) {//tab
					else sTest=ShiftMessage()+"?[\""+KeyEventArgs_KeyCodeStringLowerWas+"\"]";
				}//end if KeyEventArgs_KeyCodeStringLowerWas.Length>1
				else if (KeyEventArgs_KeyCodeStringLowerWas.Length==1) bLastKeyWasTypable=true; //if ((int)e.KeyChar>=32) {
				else sTest=ShiftMessage()+"[]";//if (KeyEventArgs_KeyCodeStringLowerWas.Length<1) {
			}//end if found a non-null InputTargetNow
			else {
				
			}
			if (!bGood) {
				RReporting.Debug("A KeyDown was ignored since there was no active tab.");
			}
		}//end FrameworkKeyDown
		///<summary>
		///Must be called after RKeyDown (remember to call RKeyUp on key up events too).
		///</summary>
		public static void FrameworkKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			bool bGood=true;
			if (ActiveTab!=null) bGood=ActiveTab.RKeyPress(e.KeyChar);
			else {
				RReporting.Debug("A KeyPress was ignored since there was no active tab.");
				bGood=false;
			}
		}
		#endregion framework keyboard input

		#region abstract gamepad i/o
		public static bool GetMappedButtonDown(int iButton) {
			return (dwButtons&RMath.Bit(iButton))!=0;
		}
		private static void SetButton(int iButton, bool bDown) {
			//TODO: map from Framework AND sdl keyboard input to here
			if (bDown) dwButtons|=RMath.Bit(iButton);
			else dwButtons&=(RMath.Bit(iButton)^RMemory.dwMask);
		}
		public static string GetMappedButtonMessage() {
			string sButtonMessage="";
			for (int iNow=0; iNow<32; iNow++) {
				if (GetMappedButtonDown(iNow)) sButtonMessage+=iNow.ToString()+" ";
			}
			return sButtonMessage;
		}
		#endregion abstract gamepad i/o

		#region abstract mouse i/o
		public static string GetMouseButtonMessage() {
			string sButtonMessage="";
			for (int iNow=0; iNow<32; iNow++) {
				if (GetMouseButtonDown(iNow)) sButtonMessage+=iNow.ToString()+" ";
			}
			return sButtonMessage;
		}
		public static bool GetMouseButtonDown() {
			return dwMouseButtons!=0;
		}
		public static bool GetMouseButtonDown(int iButton) {
			return (dwMouseButtons&RMath.Bit(iButton))!=0;
		}
		private static void SetMouseButton(int iButton, bool bDown) {
			if (bDown) dwMouseButtons|=RMath.Bit(iButton);
			else dwMouseButtons&=(RMath.Bit(iButton)^RMemory.dwMask);
		}
		#endregion abstract mouse i/o

		public static void SetMode(int iMode_Proper) {
			Console.Write("finding index for mode "+iMode_Proper.ToString()+":[");//debug only
			try {
				for (int iNow=0; iNow<modes.Count; iNow++) {
					if (modes.Element(iNow).Value==iMode_Proper) {
						SetModeByIndex(iNow);
						Console.Write(iNow);//debug only
						break;
					}
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","SetMode");
			}
			Console.WriteLine("]");//debug only
		}
		public static void SetModeByIndex(int iSetModeIndex) {
			if (iMode>=0&&iSetModeIndex!=iMode) {
				iMode=iSetModeIndex;
				OnSetMode();
			}
		}
		public static void SetMode(string sSetMode) {
			SetModeByIndex(IndexOfMode(sSetMode));
		}
		public static void SetWorkspace(int iSet) {
			if (iSet>=0&&(iSet!=iWorkspace)) {
				iWorkspace=iSet;
				OnSetWorkspace();
			}
		}
		public static void SetWorkspace(string sSet) {
			SetWorkspace(IndexOfWorkspace(sSet));
		}
		public static void SetEventObject(ref StringQ stringqueueEvents) {
			sqEvents=stringqueueEvents;
		}
		private static void OnSetWorkspace() {
			try {
				//go to the default mode for the workspace
				for (int iNow=0; iNow<modes.Count; iNow++) {
					if (modes.Element(iNow).Parent==iWorkspace) {
						SetModeByIndex(iNow);
						break;
					}
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","OnSetWorkspace");
			}
		}
		public static void SetTool(int iSet) {
			if (iSet>=0&&(iSet!=iTool)) {
				iTool=iSet;
				OnSetTool();
			}
		}
		public static void SetTool(string sSet) {
			SetTool(IndexOfTool(sSet));
		}
		private static void OnSetTool() {
			try {
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","OnSetTool");
			}
		}
		private static int iLastLUID=0;//locally-unique identifier
		private static RApplicationNodeStack workspaces=null;//tier 1
		private static RApplicationNodeStack modes=null;//tier 2
		private static RApplicationNodeStack tools=null;//tier 3
		private static RApplicationNodeStack options=null;//tier 4
		
		private static void OnSetMode() {
			iWorkspace=modes.Element(iMode).Parent;
		}
		
		
		#region collection management
		//public string Deq() {
		//	return sqEvents.Deq();
		//}
		//public static bool IsEmpty {
		//	get { return sqEvents.IsEmpty; }
		//}
		public RApplicationNode[] Workspaces() {
			return workspaces.ToArray();
		}
		public RApplicationNode[] ActiveModes() {
			return modes.ChildrenToArray(iWorkspace);
		}
		public string[] ListToolGroups() {
			StringStack sstackGroupsReturn=new StringStack();
			try {
				for (int iNow=0; iNow<tools.Count; iNow++) {
					if (tools.Element(iNow).Parent==iMode) //this is right
						sstackGroupsReturn.PushIfUnique(tools.Element(iNow).Group);
				}
			}
			catch (Exception exn) {	
				RReporting.ShowExn(exn,"","RApplication ListToolGroups");
			}
			return sstackGroupsReturn.ToArray();
		}
		public RApplicationNode[] ActiveTools(string sInGroup) {
			RApplicationNodeStack toolsReturn=new RApplicationNodeStack();
			try {
				for (int iNow=0; iNow<tools.Count; iNow++) {
					if (tools.Element(iNow).Parent==iMode
						&&tools.Element(iNow).Group==sInGroup) toolsReturn.Push(tools.Element(iNow));
				}
			}
			catch (Exception exn) {	
				RReporting.ShowExn(exn,"","RApplication ActiveTools");
			}
			return toolsReturn.ToArray();
		}
		public RApplicationNode[] ListOptions() {
			RApplicationNodeStack optionsReturn=new RApplicationNodeStack();
			try {
				for (int iNow=0; iNow<options.Count; iNow++) {
					if (options.Element(iNow).Parent==iTool) optionsReturn.Push(options.Element(iNow));
				}
			}
			catch (Exception exn) {	
				RReporting.ShowExn(exn,"","RApplication ListOptions");
			}
			return optionsReturn.ToArray();
		}
		public static void GetEvents(ref StringQ sqTo) {
			try {
				while (!sqEvents.IsEmpty) {
					sqTo.Enq(sqEvents.Deq());
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","GetAllEvents");
			}
		}
		public const string RootMode="";
		public const string MainToolGroup="main";
		public static string VarMessageStyleOperatorAndValue(RApplicationNode val, bool bShowStringIfValid) {
			string sMsg;//=VariableMessage(byarrToHex);
			sMsg=VarMessage(val,bShowStringIfValid);
			if (!bShowStringIfValid && RString.IsNumeric(sMsg,false,false)) sMsg=".Length:"+sMsg;
			else sMsg=":"+sMsg;
			return sMsg;
		}
		public static string VarMessage(RApplicationNode val, bool bShowStringIfValid) {
			try {
				return (val!=null)  
					?  ( bShowStringIfValid ? ("\""+val.ToString()+"\"") : val.ToString().Length.ToString() )
					:  "null";
			}
			catch {//do not report this
				return "incorrectly-initialized-var";
			}
		}
		public static bool PushWorkspace(RApplicationNode valNew) {
			bool bGood=false;
			try { bGood=workspaces.Push(valNew); }
			catch (Exception exn) { RReporting.ShowExn(exn,"","PushWorkspace"); }
			return bGood;
		}
		public static bool PushMode(RApplicationNode valNew) {
			bool bGood=false;
			try { bGood=modes.Push(valNew); }
			catch (Exception exn) { RReporting.ShowExn(exn,"","PushMode"); }
			return bGood;
		}
		public static bool PushTool(RApplicationNode valNew) {
			bool bGood=false;
			try { bGood=tools.Push(valNew); }
			catch (Exception exn) { RReporting.ShowExn(exn,"","PushTool"); }
			return bGood;
		}
		public static bool PushOption(RApplicationNode valNew) {
			bool bGood=false;
			try { bGood=options.Push(valNew); }
			catch (Exception exn) { RReporting.ShowExn(exn,"","PushOption"); }
			return bGood;
		}
		/*
		public static bool SetByRef(int iAt, RApplicationNode valNew) {
			bool bGood=true;
			if (iAt==Maximum) SetFuzzyMaximum(iAt);
			else if (iAt>Maximum) {
				RReporting.Warning("Setting RApplication Maximum to arbitrary index {iElements:"+iElements.ToString()+"; iAt:"+iAt.ToString()+"; sName:"+sName+"; }");
				SetFuzzyMaximum(iAt);
			}
			if (iAt<Maximum&&iAt>=0) {
				nodearr[iAt]=valNew;
				if (iAt>iElements) iElements=iAt+1;//warning already shown above
				else if (iAt==iElements) iElements++;
			}
			else {
				bGood=false;
				RReporting.ShowErr("Could not increase maximum abstract interface nodes.","RApplicationNode SetByRef","setting abstract interface node by reference {valNew"+VarMessageStyleOperatorAndValue(valNew,true)+"; iAt:"+iAt.ToString()+"; iElements:"+iElements.ToString()+"; Maximum:"+Maximum.ToString()+"}");
			}
			return bGood;
		}
		*/
		#endregion collection management
		
		private static string GetLUID() {
			iLastLUID++;
			return "<!--LUID:"+(iLastLUID-1).ToString()+"-->";
		}
		public static void OnChangeMode() {
			sqEvents.Enq("onchangemode");//TODO: is this a good time to push this event?
		}
		
		public static void AddWorkspace(string sName, string sCaption) {
			AddWorkspace(sName,sCaption,"","");
		}
		public static void AddWorkspace(string sName, string sCaption, string sToolTip, string sTechTip) {
			try {
				RApplicationNode nodeNew=new RApplicationNode(0, sName, sCaption, sToolTip, sTechTip, RApplicationNode.TypeWorkspace);
				nodeLastCreated=nodeNew;
				//nodeNew.Maximum=iSetModeNumber;
				//nodeNew.Value=iSetModeNumber;
				PushWorkspace(nodeNew);
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"", "RApplication AddMode");
			}
		}
		public static void AddMode(string sParentName, int iSetModeNumber, string sName) {
			AddMode(sParentName, iSetModeNumber, sName, sName, "", "");
		}
		public static void AddMode(string sParentName, int iSetModeNumber, string sName, string sCaption) {
			AddMode(sParentName, iSetModeNumber, sName, sCaption, "", "");
		}
		/// <summary>
		/// Mode, i.e. MainMenu, Game, Editor
		/// </summary>
		public static void AddMode(string sParentName, int iSetModeNumber, string sName, string sCaption, string sToolTip, string sTechTip) {
			try {
				int iParent=IndexOfWorkspace(sParentName);
				//TODO: finish this - AddMode
				RApplicationNode nodeNew=new RApplicationNode(iParent, sName, sCaption, sToolTip, sTechTip, RApplicationNode.TypeMode);
				nodeLastCreated=nodeNew;
				nodeNew.MaxValue=iSetModeNumber;
				nodeNew.Value=iSetModeNumber;
				Console.WriteLine("Created mode "+nodeNew.Value+" at index "+modes.Count.ToString());
				PushMode(nodeNew);
				IncrementWorkspaceChildCount(iParent);
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"", "RApplication AddMode");
			}
		}
		public static int IndexOfWorkspace(string sOfName) {
			int iReturn=-1;
			try {
				for (int iNow=0; iNow<workspaces.Count; iNow++) {
					if (workspaces.Element(iNow).Name==sOfName) return iNow;
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"", "RApplication IndexOfWorkspace");
			}
			if (iReturn==-1) RReporting.Warning("RApplication IndexOfWorkspace not found {sOfName:"+sOfName+"}");
			return iReturn;
		}
		public static int IndexOfMode(string sOfName) {
			int iReturn=-1;
			try {
				for (int iNow=0; iNow<modes.Count; iNow++) {
					if (modes.Element(iNow).Name==sOfName) return iNow;
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"", "RApplication IndexOfMode");
			}
			if (iReturn==-1) RReporting.Warning("RApplication IndexOfMode not found {sOfName:"+sOfName+"}");
			return iReturn;
		}
		public static int IndexOfTool(string sOfName) {
			int iReturn=-1;
			try {
				for (int iNow=0; iNow<tools.Count; iNow++) {
					if (tools.Element(iNow).Name==sOfName) return iNow;
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"", "RApplication IndexOfTool");
			}
			if (iReturn==-1) RReporting.Warning("RApplication IndexOfTool not found {sOfName:"+sOfName+"}");
			return iReturn;
		}
		public static int IndexOfOption(string sOfName) {
			int iReturn=-1;
			try {
				for (int iNow=0; iNow<options.Count; iNow++) {
					if (options.Element(iNow).Name==sOfName) return iNow;
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"", "RApplication IndexOfOption");
			}
			if (iReturn==-1) RReporting.Warning("RApplication IndexOfOption not found {sOfName:"+sOfName+"}");
			return iReturn;
		}
		public static void AddTool(string sModeParent, string sName) {
			AddTool(sModeParent, MainToolGroup, sName, sName, "","");
		}
		public static void AddTool(string sModeParent, string sName, string sCaption) {
			AddTool(sModeParent, MainToolGroup, sName, sCaption, "","");
		}
		/// <summary>
		/// Menubar, Toolbox, StatusBar
		/// </summary>
		public static void AddTool(string sModeParent, string sToolGroup, string sName, string sCaption, string sToolTip,string sTechTip) {
			try {
				int iParent=IndexOfMode(sModeParent);
				//TODO: finish this - AddTool
				RApplicationNode nodeNew=new RApplicationNode(iParent, sName, sCaption, sToolTip, sTechTip, RApplicationNode.TypeTool, sToolGroup);
				nodeLastCreated=nodeNew;
				PushTool(nodeNew);
				IncrementModeChildCount(iParent);
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"", "RApplication AddMode");
			}
		}
		
		public static void AddOption(string sToolParent, string sName) {
			AddOption(sToolParent, GetLUID(), sName, sName, "");
		}
		public static void AddOption(string sToolParent, string sName, string sCaption) {
			AddOption(sToolParent, GetLUID(), sName, sCaption, "");
		}
		public static void AddOption_ForceAsCheckBox(string sToolParent, string sName, string sCaption, string sToolTip, string sTechTip) {
			AddOption(sToolParent, GetLUID(), sName, sCaption, sToolTip, sTechTip);
		}
		public static void AddOption(string sToolParent, string sOptionChooser_Group, string sName, string sCaption, string sToolTip) {
			AddOption(sToolParent,sOptionChooser_Group,sName,sCaption,sToolTip,"");
		}
		public static void AddOption(string sToolParent, string sOptionChooser_Group, string sName, string sCaption, string sToolTip, string sTechTip) {
			//TODO: finish this - AddOption
			int iParent=IndexOfTool(sToolParent);
			RApplicationNode nodeNew=new RApplicationNode(iParent, sName, sCaption, sToolTip, sTechTip, RApplicationNode.TypeOption, sOptionChooser_Group);
			IncrementToolChildCount(iParent);
			PushOption(nodeNew);
		}
		public static void PrepareForUse() {
			//TODO: finish this - Prepare RApplication for use
			//--calculate ChildCount
			//--set bRadio for self and siblings if any siblings are found (only if !bRadio already!) (by checking for ones with same non-null non-blank value for sGroup)
			//--select option if bRadio (first check whether .Enabled)
			//NOTES: tools is an RApplicationNodeStack
			for (int iOperand1=0; iOperand1<tools.Count; iOperand1++) {
				for (int iOperand2=0; iOperand2<tools.Count; iOperand2++) {
					if (tools.Element(iOperand1)!=null && tools.Element(iOperand2)!=null
						&& iOperand1!=iOperand2
						&& tools.Element(iOperand1).OptionType==RApplicationNode.OptionTypeChooser
						&& tools.Element(iOperand1).Group!=null && tools.Element(iOperand1).Group!=""
						&& tools.Element(iOperand2).Group!=null && tools.Element(iOperand2).Group!=""
						&&tools.Element(iOperand1).Parent==tools.Element(iOperand2).Parent
						&&tools.Element(iOperand1).Group==tools.Element(iOperand2).Group
						) {
							tools.Element(iOperand1).bRadio=true;
							tools.Element(iOperand2).bRadio=true;
						}
				}
			}
		}
		public static void IncrementToolChildCount(int iParent) {
			try {
				if (iParent>=0) tools.Element(iParent).ChildCount++;
				else RReporting.Warning("Tried to increment nonexistant ancestor.");
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","RApplication IncrementWorkspaceChildCount");
			}
		}
		public static void IncrementModeChildCount(int iParent) {
			try {
				if (iParent>=0) modes.Element(iParent).ChildCount++;
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","RApplication IncrementWorkspaceChildCount");
			}
		}
		public static void IncrementWorkspaceChildCount(int iParent) {
			try {
				if (iParent>=0) workspaces.Element(iParent).ChildCount++;
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","RApplication IncrementWorkspaceChildCount");
			}
		}
	}///end RApplication
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	public class RApplicationNodeStack { //pack Stack -- array, order left(First) to right(Last)
		private RApplicationNode[] nodearr=null;
		private int Maximum {
			get {
				return (nodearr==null)?0:nodearr.Length;
			}
			set {
				RApplicationNode.Redim(ref nodearr,value);
			}
		}
		private int iCount;
		private int LastIndex {	get { return iCount-1; } }
		private int NewIndex { get  { return iCount; } }
		//public bool IsFull { get { return (iCount>=Maximum) ? true : false; } }
		public bool IsEmpty { get { return (iCount<=0) ? true : false ; } }
		public RApplicationNode Element(int iElement) {
			return (iElement<iCount&&iElement>=0&&nodearr!=null)?nodearr[iElement]:null;
		}
		//public RApplicationNode Element(string sByName) {
		//	int index=IndexOf(sByName);
		//	if (index>=0) return nodearr[iNow];
		//	else return null;
		//}
		public int Count {
			get {
				return iCount;
			}
		}
		///<summary>
		///
		///</summary>
		public int CountInstancesI(string sCaseInsensitiveSearch) {
			int iReturn=0;
			sCaseInsensitiveSearch=sCaseInsensitiveSearch.ToLower();
			for (int iNow=0; iNow<iCount; iNow++) {
				if (nodearr[iNow].Name.ToLower()==sCaseInsensitiveSearch) iReturn++;
			}
			return iReturn;
		}
		public int CountInstances(string sCaseSensitiveSearch) {
			int iReturn=0;
			for (int iNow=0; iNow<iCount; iNow++) {
				if (nodearr[iNow].Name==sCaseSensitiveSearch) iReturn++;
			}
			return iReturn;
		}
		public bool ExistsI(string sCaseInsensitiveSearch) {
			return CountInstancesI(sCaseInsensitiveSearch)>0;
		}
		public bool Exists(string sCaseInsensitiveSearch) {
			return CountInstances(sCaseInsensitiveSearch)>0;
		}
		public RApplicationNodeStack() { //Constructor
			//int iDefaultSize=100;
			//TODO: settings.GetOrCreate(ref iDefaultSize,"StringStackDefaultStartSize");
			//Init(iDefaultSize);
			RReporting.ShowErr("Program should not have instantiated an RApplication object.  The object should only be used statically.");
		}
		//public RApplicationNodeStack(int iSetMax) { //Constructor
		//	Init(iSetMax);
		//}
// 		private void Init(int iSetMax) { //always called by Constructor
// 			if (iSetMax<0) RReporting.Warning("RApplicationNodeStack initialized with negative number so it will be set to a default.");
// 			else if (iSetMax==0) RReporting.Warning("RApplicationNodeStack initialized with zero so it will be set to a default.");
// 			if (iSetMax<=0) iSetMax=1;
// 			Maximum=iSetMax;
// 			iCount=0;
// 			if (nodearr==null) RReporting.ShowErr("Stack constructor couldn't initialize nodearr");
// 		}
		public void Clear() {
			iCount=0;
			for (int iNow=0; iNow<nodearr.Length; iNow++) {
				nodearr[iNow]=null;
			}
		}
		public void ClearFastWithoutFreeingMemory() {
			iCount=0;
		}
		public void SetFuzzyMaximumByLocation(int iLoc) {
			int iNew=iLoc+iLoc/2+1;
			if (iNew>Maximum) Maximum=iNew;
		}
		public bool PushIfUnique(RApplicationNode nodeAdd) {
			if (!Exists(nodeAdd.Name)) return Push(nodeAdd); 
			else return false;
		}
		public bool Push(RApplicationNode nodeAdd) {
			//if (!IsFull) {
			try {
				if (NewIndex>=Maximum) SetFuzzyMaximumByLocation(NewIndex);
				nodearr[NewIndex]=nodeAdd;
				iCount++;
				//sLogLine="debug enq iCount="+iCount.ToString();
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"setting nodearr","RApplicationNodeStack Push("+((nodeAdd==null)?"null RApplicationNode":"non-null")+"){NewIndex:"+NewIndex.ToString()+"}");
				return false;
			}
			return true;
			//}
			//else {
			//	if (sAdd==null) sAdd="";
			//	RReporting.ShowErr("StringStack is full, can't push \""+sAdd+"\"! ( "+iCount.ToString()+" strings already used)","StringStack Push("+((sAdd==null)?"null string":"non-null")+")");
			//	return false;
			//}
		}
		public RApplicationNode Pop() {
			//sLogLine=("debug deq iCount="+iCount.ToString()+" and "+(IsEmpty?"is":"is not")+" empty.");
			if (IsEmpty) {
				//RReporting.ShowErr("no strings to return so returned null","StringStack Pop");
				return null;
			}
			int iReturn = LastIndex;
			iCount--;
			return nodearr[iReturn];
		}
		public RApplicationNode[] ToArray() {
			RApplicationNode[] nodearrReturn=null;
			try {
				if (iCount>0) nodearrReturn=new RApplicationNode[iCount];
				for (int iNow=0; iNow<iCount; iNow++) {
					nodearrReturn[iNow]=nodearr[iNow];
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","RApplicationNodeStack ToArray");
			}
			return nodearrReturn;
		}
		public RApplicationNode[] ChildrenToArray(int iParent) {
			RApplicationNodeStack nodesReturn=null;
			try {
				nodesReturn=new RApplicationNodeStack();
				for (int iNow=0; iNow<iCount; iNow++) {
					if (nodearr[iNow].Parent==iParent) nodesReturn.Push(nodearr[iNow]);
				}
			}
			catch (Exception exn) {
				RReporting.ShowExn(exn,"","RApplicationNodeStack ChildrenToArray");
			}
			return nodesReturn.ToArray();
		}
	}///end RApplicationNodeStack
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	/// <summary>
	/// An interface abstractor node. See also RApplication.
	/// </summary>
	public class RApplicationNode {
		#region static variables
		public static readonly string[] sarrType=new string[]{"uninitialized-RApplicationNode-Type","workspace","mode","tool","option"};
		public const int TypeUninitialized=0;
		public const int TypeWorkspace=1;
		public const int TypeMode=2;//indicates that the value is a mode integer
		public const int TypeTool=3;
		public const int TypeOption=4;
		
		public static readonly string[] sarrOptionType=new string[]{"uninitialized-RApplicationNode-OptionType","chooser","slider","text","numericupdown"};
		public const int OptionTypeUninitialized=0;//boolean if no others in group
		public const int OptionTypeChooser=1;//boolean if no others in group
		public const int OptionTypeSlider=2;
		public const int OptionTypeText=3;
		public const int OptionTypeNumericUpDown=4;
		#endregion static variables

		#region variables
		public int ChildCount=0;
		private int iType=TypeUninitialized;
		private int iOptionType=OptionTypeUninitialized;//only used if option
		private bool bMultiLine=false;//only used if OptionTypeText
		//private string Parent="";//may be same as mode //TODO: finish this--if same as mode, make the checkbox
		private int iParent=-1;
		public int Parent { get {return iParent;} }
		private bool bEnabled=true;
		private int iValue=0;//used as mode integer if TypeMode; else as value if numeric (slider/updown)
		private string sValue="";//Text property (see Text accessor below)
		private int iMinValue=0;//only used if numeric (slider/updown)
		private int iMaxValue=255;//only used if numeric (slider/updown)
		private string sName="";
		private string sCaption="";
		private string sToolTip="";
		private string sTechTip="";
		private string sGroup="";//used as group for TypeTool; group for optiongroup
		public bool bRadio=false;//automatically set to true of more than one option with same group
		/// <summary>
		/// If true, the value will be shown as ((iValue-iMinValue)/(iMaxValue-iMinValue))*100.0
		/// </summary>
		bool bAsPercent;
		public bool Enabled {
			get { return bEnabled; }
			set { bEnabled=value; }
		}
		public string Name {
			get { return sName; }
			set { sName=value; }
		}
		public bool Checked {
			get { return iValue!=0; }
			set { iValue=(value)?1:0; }
		}
		public int Value {
			get { return iValue; }
			set { iValue=value; LimitValue(); }
		}
		public string Caption {
			get { return sCaption; }
			set { sCaption=value; }
		}
		public string Text {
			get { return sValue; }
			set { sValue=value; }
		}
		public bool IsLeaf {
			get { return ChildCount==0; }
		}
		public string ToolTip {
			get { return sToolTip; }
			set { sToolTip=value; }
		}
		public string TechTip {
			get { return sTechTip; }
			set { sTechTip=value; }
		}
		public int MinValue {
			get { return iMinValue; }
			set { iMinValue=value; LimitValue(); }
		}
		public int MaxValue {
			get { return iMaxValue; }
			set { iMaxValue=value; LimitValue(); }
		}
		public string Group {
			get { return sGroup; }
		}
		public int OptionType {
			get { return iOptionType; }
		}
		#endregion variables
		
		#region constructors
		public RApplicationNode() {
			RReporting.Warning("Default RApplicationNode constructor was used.");
		}
		public RApplicationNode(int iSetParent, string sSetName, string sSetCaption, string sSetToolTip, string sSetTechTip, int iSetType) {
			Init(iSetParent,sSetName,sSetCaption,sSetToolTip,sSetTechTip,iSetType,OptionTypeUninitialized,"");
			if (iSetType==TypeOption) RReporting.Warning("The OptionType overload of the RApplicationNode constructor must be used if the RApplicationNode.Type is TypeOption.");
			if (iSetType==TypeTool) RReporting.Warning("The SetGroup overload of the RApplicationNode constructor must be used if the RApplicationNode.Type is TypeOption.");
		}
		public RApplicationNode(int iSetParent, string sSetName, string sSetCaption, string sSetToolTip, string sSetTechTip, int iSetType, int iSetOptionType, string sSetGroup) {
			Init(iSetParent,sSetName,sSetCaption,sSetToolTip,sSetTechTip,iSetType,iSetOptionType,sSetGroup);
			if (iSetType!=TypeOption&&iSetType!=TypeTool) RReporting.Warning("Only RApplication.TypeOption should use the iSetOptionType constructor overload");
		}
		public RApplicationNode(int iSetParent, string sSetName, string sSetCaption, string sSetToolTip, string sSetTechTip, int iSetType, string SetGroup) {
			Init(iSetParent,sSetName,sSetCaption,sSetToolTip,sSetTechTip,iSetType,OptionTypeUninitialized,SetGroup);
			if (iSetType!=TypeTool) RReporting.Warning("Only RApplication.TypeTool should use the SetGroup constructor overload");
		}
		private void Init(int iSetParent, string sSetName, string sSetCaption, string sSetToolTip, string sSetTechTip, int iSetType, int iSetOptionType, string SetGroup) {
			iParent=iSetParent;
			sName=sSetName;
			sCaption=sSetCaption;
			sToolTip=sSetToolTip;
			sTechTip=sSetTechTip;
			bEnabled=true;
			iType=iSetType;
			iOptionType=iSetOptionType;
			sGroup=SetGroup;
		}
		#endregion constructors
		
		#region utilities
		private void LimitValue() {
			if (iValue>iMaxValue) iValue=iMaxValue;
			else if (iValue<iMinValue) iValue=iMinValue;
		}
		public static string TypeToString(int Type_x) {
			string sReturn="uninitialized-RApplicationNode.Type";
			try {
				sReturn=sarrType[Type_x];
			}
			catch {
				sReturn="nonexistent-RApplicationNode.Type["+Type_x.ToString()+"]";
			}
			return sReturn;
		}
		public static string OptionTypeToString(int OptionType_x) {
			string sReturn="uninitialized-RApplicationNode.OptionType";
			try {
				sReturn=sarrType[OptionType_x];
			}
			catch {
				sReturn="nonexistent-RApplicationNode.OptionType("+OptionType_x.ToString()+")";
			}
			return sReturn;
		}
		public float ToPercentTo1F() {
			return ( ( RConvert.ToFloat(iValue-iMinValue)
				/ RConvert.ToFloat(iMaxValue-iMinValue) ) );
		}
		public double ToPercentTo1D() {
			return ( ( RConvert.ToDouble(iValue-iMinValue)
				/ RConvert.ToDouble(iMaxValue-iMinValue) ) );
		}
		public string ToPercentString(int iPlaces) {
			string sReturn=RMath.RemoveExpNotation( (ToPercentTo1D()*100.0).ToString() );
			int iDot=sReturn.IndexOf(".");
			if (iDot>=0) {
				if (iPlaces>0) {
					int iPlacesNow=(sReturn.Length-1)-iDot;
					if (iPlacesNow>iPlaces) sReturn=RString.SafeSubstring(sReturn, 0, sReturn.Length-(iPlacesNow-iPlaces));
				}
				else {//convert to no decimal places
					if (iDot>0) sReturn=RString.SafeSubstring(sReturn,0,iDot);
					else sReturn="0";
				}
			}
			return sReturn+"%";
		}//end ToPercentString
		public static int SafeLength(RApplicationNode[] valarrNow) {
			int iReturn=0;
			try {
				if (valarrNow!=null) iReturn=valarrNow.Length;
			}
			catch {
			}
			return iReturn;
		}
		/// <summary>
		/// Sets size, preserving data
		/// </summary>
		public static bool Redim(ref RApplicationNode[] valarrNow, int iSetSize) {
			bool bGood=false;
			if (iSetSize!=SafeLength(valarrNow)) {
				if (iSetSize<=0) { valarrNow=null; bGood=true; }
				else {
					try {
						//bool bGood=false;
						RApplicationNode[] valarrNew=new RApplicationNode[iSetSize];
						for (int iNow=0; iNow<valarrNew.Length; iNow++) {
							if (iNow<SafeLength(valarrNow)) valarrNew[iNow]=valarrNow[iNow];
							else valarrNew[iNow]=null;//Var.Create("",TypeNULL);
						}
						valarrNow=valarrNew;
						//bGood=true;
						//if (!bGood) RReporting.ShowErr("No vars were found while trying to set MaximumSeq!");
						bGood=true;
					}
					catch (Exception exn) {
						bGood=false;
						RReporting.ShowExn(exn,"setting var maximum","Var Redim");
					}
				}
			}
			else bGood=true;
			return bGood;
		}//end Redim
		#endregion utilities
	}///end RApplicationNode
}//end namespace