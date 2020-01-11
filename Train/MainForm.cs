using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Train.Trains;
using System.Text;
using Train.Utilities;
using Train.ShowMsg;
using Train.Messages;
using Train.Packets;
using Train.XmlResolve;
using Train.Data;
using Train.MessageHandlers;
using System.IO;
using System.Net.Sockets;

namespace Train
{
    public partial class MainForm : Form
    {
        private DriverConsolerState driverConsoler = DriverConsolerState.GetDriverConsolerA();
        public DriverConsolerState DriverConsoler { get { return driverConsoler; } }
        private CircularQueue<ListViewContent> recvMsgQueue = new CircularQueue<ListViewContent>();
        private CircularQueue<ListViewContent> sendMsgQueue = new CircularQueue<ListViewContent>();
        private volatile bool isISDNIFConnected = false;//to mark the status of connection with ISDNIF 
        private volatile bool isRBCConnected = false;
        public bool IsRBCConnected { get { return isRBCConnected; } }
        private volatile bool isNRBCConnected = false;
        public bool IsNRBCConnected { get { return isNRBCConnected; } }
        private const int BOUNDARY_POINT = 57406;   // boundary of RBC1 and RBC2,i.e. RBC and NRBC
        Database database = new Database();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            database.Init();
            StartTrain();
            StartComm();
            rbSB.Checked = true;    // 一开始列车处于SB模式
            rbCTCS_2.Checked = true;
            rbDown.Checked = true;
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (database != null) database.Close();
            Communication.Close();
        }

        private _CommType CheckRBCRegion()
        {
            return trainState.TrainLocation.LeftLoc > BOUNDARY_POINT ? _CommType.NRBC : _CommType.RBC;
        }

        #region 菜单项
        private void ConnectTSMI_Click(object sender, EventArgs e)
        {
            if (sender == rBCToolStripMenuItem)
            {
                Communication.Connect(_CommType.RBC);
                StartRecvMsgModule(_CommType.RBC);
            }
            else if (sender == nRBCToolStripMenuItem)
            {
                Communication.Connect(_CommType.NRBC);
                StartRecvMsgModule(_CommType.NRBC);
            }
        }

        private void DisconnectTSMI_Click(object sender, EventArgs e)
        {
            if (sender == disRBCToolStripMenuItem)
            {
                Message150 m150 = new Message150();
                m150.SetPacket0or1(TrainDynamics.GetPacket0());
                SendMsg(m150, _CommType.RBC);
                Communication.Disconnect(_CommType.RBC);
                isISDNIFConnected = false;
                isRBCConnected = false;
            }
            else if (sender == disNRBCToolStripMenuItem)
            {
                Communication.Disconnect(_CommType.NRBC);
                isNRBCConnected = false;
            }
        }


        private void 列车位置设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Trains.SetLocForm(trainState.TrainLocation).Show();
        }

        #region 故障注入
        private void 版本不兼容ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FaultInjection.IsVersionCompatible = !FaultInjection.IsVersionCompatible;
            版本不兼容ToolStripMenuItem.Checked = ! FaultInjection.IsVersionCompatible;
        }

        private void 不发送任何消息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FaultInjection.IsNoMessageSent = !FaultInjection.IsNoMessageSent;
            不发送任何消息ToolStripMenuItem.Checked = FaultInjection.IsNoMessageSent;
        }

        private void soM位置未知ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool newValue = !soM位置未知ToolStripMenuItem.Checked;
            soM位置未知ToolStripMenuItem.Checked = newValue;
            if (newValue)
            {
                soM位置无效ToolStripMenuItem.Checked = false; // this two option cannot be true at the same time
                FaultInjection.QStatus = _Q_STATUS.UNKNOWN;
            }
            else FaultInjection.QStatus = _Q_STATUS.VALID;
        }

        private void soM位置无效ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool newValue = !soM位置无效ToolStripMenuItem.Checked;
            soM位置无效ToolStripMenuItem.Checked = newValue;
            if (newValue)
            {
                soM位置未知ToolStripMenuItem.Checked = false;
                FaultInjection.QStatus = _Q_STATUS.INVALID;
            }
            else FaultInjection.QStatus = _Q_STATUS.VALID;
        }

        private void 不执行等级转换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FaultInjection.IsNoLevelTr = !FaultInjection.IsNoLevelTr;
            不执行等级转换ToolStripMenuItem.Checked = FaultInjection.IsNoLevelTr;
        }


        #endregion
        //发送一次指定消息
        private void SendMessagetTSMI_Click(object sender, EventArgs e)
        {
            _CommType commType = CheckRBCRegion();
            if(sender == Message129ToolStripMenuItem)
            {
                Message129 m129 = new Message129();
                m129.SetPacket0or1(TrainDynamics.GetPacket0());
                //Packet011 are some static configurations,so don't need to set its field here.
                SendMsg(m129, commType);
            }
            if(sender == Msg132NoPacketToolStripMenuItem)
            {
                Message132 m132 = new Message132();
                if (trainDynamic == null) return;
                m132.SetPacket0or1(TrainDynamics.GetPacket0());
                SendMsg(m132, commType);
            }
            if (sender == Msg132Packet9ToolStripMenuItem)
            {
                Message132 m132 = new Message132();
                if (trainDynamic == null) return;
                m132.SetPacket0or1(TrainDynamics.GetPacket0());
                m132.SetAlternativePacket(new Packet009());  //设置可选择的信息包9
                SendMsg(m132, commType);
            }
            if (sender == Msg136NoPacketToolStripMenuItem)
            {
                Message136 m136 = new Message136();
                if (trainDynamic == null) return;
                m136.SetPacket0or1(TrainDynamics.GetPacket0());
                SendMsg(m136, commType);
            }
            if (sender == Message150ToolStripMenuItem)
            {
                Message150 m150 = new Message150();
                if (trainDynamic == null) return;
                m150.SetPacket0or1(TrainDynamics.GetPacket0());
                SendMsg(m150, commType);
            }
            if (sender == Message155ToolStripMenuItem)
            {
                Message155 m155 = new Message155();
                SendMsg(m155, commType);
            }
            if(sender == Message156ToolStripMenuItem)
            {
                Message156 m156 = new Message156();
                SendMsg(m156, commType);
            }
            if(sender == Msg157NoPacketToolStripMenuItem)
            {
                Message157 m157 = new Message157();
                if (trainDynamic == null) return;
                m157.SetPacket0or1(TrainDynamics.GetPacket0());
                SendMsg(m157, commType);
            }
            if(sender == Msg159NoPacketToolStripMenuItem)
            {
                Message159 m159 = new Message159();
                SendMsg(m159, commType);
            }
            if(sender == Msg159Packet3ToolStripMenuItem)
            {
                Message159 m159 = new Message159();
                m159.SetAlternativePacket(new Packet003Train());
                SendMsg(m159, commType);
            }
        }
        #endregion

        #region 主界面
        // RadioButton.Checked状态由true变为false，或是由false变为true，都会触发此事件
        private void rbCabChoose_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = ((RadioButton)sender);
            if (rb.Checked == false) return;
            String name = rb.Name;
            DriverConsolerState lastState = driverConsoler;
            if (name.Equals("rbCabA"))
                driverConsoler = DriverConsolerState.GetDriverConsolerA().CopyOf(lastState);
            else if (name.Equals("rbCabB"))
                driverConsoler = DriverConsolerState.GetDriverConsolerB().CopyOf(lastState);
            else driverConsoler = DriverConsolerState.GetNULL().CopyOf(lastState);
        }

        private void rbDirectionChoose_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = ((RadioButton)sender);
            if (rb.Checked == false) return;
            String name = rb.Name;
            if (name.Contains("Forward"))
                driverConsoler.DriveDirection = DriveDirection.RMF;
            else if (name.Contains("Backward"))
                driverConsoler.DriveDirection = DriveDirection.RMR;
            else driverConsoler.DriveDirection = DriveDirection.ZERO;
        }

        private void rbLine_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = ((RadioButton)sender);
            if (rb.Checked == false) return;
            String name = rb.Name;
            if (name.Contains("Up"))
                driverConsoler.Line = _Line.UP;
            else
                driverConsoler.Line = _Line.DOWN;
        }

        //需要在EB_MH 中调用这个函数
        public void rbWorkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is _M_MODE)
            {
                driverConsoler.WorkMode = (_M_MODE)sender;
                foreach(Control c in gbWorkMode.Controls)
                {
                    if (c is RadioButton==false) continue;
                    RadioButton rb = (RadioButton)c;
                    if (rb.Name.Contains(sender.ToString()))
                    {
                        rb.Checked = true;
                        break;
                    }
                }
            }
            else    // sender is RadioButton
            {
                RadioButton rb = (RadioButton)sender;
                if (rb.Checked == false) // 由从true变为false的那个RadioButton产生的事件，直接返回
                {
                    if (rbSH.Checked && driverConsoler.WorkMode != _M_MODE.SH)
                    {
                        rb.Checked = true;
                        RequestForSH(); // request for Shunting mode
                    }
                    return;
                } 
               
                foreach (_M_MODE wm in Enum.GetValues(typeof(_M_MODE)))
                {
                    if (rb.Name.Contains(wm.ToString()))
                    {
                        driverConsoler.WorkMode = wm;
                        break;
                    }
                }
            }
            if (driverConsoler.WorkMode == _M_MODE.TR)    // 需要询问司机是否进入PT模式
            {
                AskSwitchToPT();
            }
        }

        //需要在MA_MH中调用这个函数
        public void rbControlLevel_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is _ControlLevel)
            {
                driverConsoler.ControlLevel = (_ControlLevel)sender;
                foreach (Control c in gbControlLevel.Controls)
                {
                    if (c is RadioButton == false) continue;
                    RadioButton rb = (RadioButton)c;
                    if (rb.Name.Contains(sender.ToString()))
                    {
                        rb.Checked = true;
                        break;
                    }
                }
            }
            else
            {
                RadioButton rb = (RadioButton)sender;
                if (rb.Checked == false) return;    //由从true变为false的那个RadioButton产生的事件，直接返回
                foreach (_ControlLevel cl in Enum.GetValues(typeof(_ControlLevel)))
                    if (rb.Name.Contains(cl.ToString()))
                    {
                        driverConsoler.ControlLevel = cl;
                        break;
                    }
            }
            //进入C2等级，ATP模式变为SN
            //if (driverConsoler.ControlLevel == _ControlLevel.CTCS_2)
            //    rbWorkMode_CheckedChanged(rbSL, null);
        }
        //点击此按钮表示关闭驾驶台，列车进入任务结束流程
        private void btnEoMButton_Click(object sender, EventArgs e)
        {
            _M_MODE wm = driverConsoler.WorkMode;
            //如果当前在冒进模式或冒进后模式，则模式不变
            if (wm == _M_MODE.PT || wm == _M_MODE.TR)
                return;
            driverConsoler.WorkMode = _M_MODE.SB;//进入SB模式
            if (wm == _M_MODE.SH) //如果在SH模式，则不注销
                return;
            //其它情况下，执行注销过程
            Message150 m150 = new Message150();
            m150.SetPacket0or1(TrainDynamics.GetPacket0());
            _CommType commType = CheckRBCRegion();
            SendMsg(m150, commType);
            //列车手动进入C2等级导致的注销过程由RBC发起，不需要在这里写
        }

        private void btnEBButton_Click(object sender, EventArgs e)
        {
            bool b = !driverConsoler.EBStatus;
            driverConsoler.EBStatus = b;
            btnEBButton.BackColor = b ? Color.Red : Control.DefaultBackColor;
        }

        private void trackBarSteer_Scroll(object sender, EventArgs e)
        {
            driverConsoler.SteerValue = trackBarSteer.Value - 10;
        }
        
        private void RequestForSH()
        {
            Message130 m130 = new Message130();
            m130.SetPacket0or1(TrainDynamics.GetPacket0());
            SendMsg(m130, _CommType.RBC);
        }

        private void AskSwitchToPT()
        {
            DialogResult dr = MessageBox.Show("是否进入PT模式？","",MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
                rbWorkMode_CheckedChanged(_M_MODE.PT, null);
        }
        #endregion

        #region 列车运动模块
        private TrainState trainState = new TrainState();
        public TrainState GetTrainState() { return trainState; }
        private Thread trainDynamicThread;
        private TrainDynamics trainDynamic = new TrainDynamics();
        public TrainDynamics TrainDynamic { get{ return trainDynamic; } }
        //列车界面更新线程
        private Thread updateVehicleInterfaceThread;
        public void StartTrain()
        {
            //列车运动线程
            trainDynamicThread = new Thread(TrainDynamicFun);
            trainDynamicThread.IsBackground = true;
            trainDynamicThread.Priority = ThreadPriority.Highest;
            trainDynamicThread.Start();
            //列车界面更新线程
            updateVehicleInterfaceThread = new Thread(UpdateVehicleInterfaceFun);
            updateVehicleInterfaceThread.IsBackground = true;
            updateVehicleInterfaceThread.Start();
        }
        public void TrainDynamicFun()
        {
            DriveDirection driveDirection;
            bool islefthead = false;
            double steerValue = 0;
            while (true)
            {
                if (driverConsoler == DriverConsolerState.GetNULL())
                {
                    Thread.Sleep(100);
                    continue;
                }
                islefthead = (driverConsoler == DriverConsolerState.GetDriverConsolerA());
                driveDirection = driverConsoler.DriveDirection;
                steerValue = driverConsoler.SteerValue;
                //  计算列车运动参量的函数
                if (trainDynamic != null)
                    trainDynamic.OperatingVehicle(islefthead, driverConsoler, trainState);
                Thread.Sleep(100);
            }
        }

        public delegate void UpdateVehicleInterfaceEventHandler();
        public event UpdateVehicleInterfaceEventHandler updateVehicleInterface;
        public void UpdateVehicleInterface()
        {
            #region 速度、加速度
            if (true)
            {
                bool b1 = cbManualSpeed.Checked, b2 = cbManualAccSpeed.Checked;
                nudSpeed.Enabled = cbManualSpeed.Checked;
                nudAccSpeed.Enabled = cbManualAccSpeed.Checked;
                trainState.BManualSpeed = cbManualSpeed.Checked;
                trainState.BManualAccSpeed = cbManualAccSpeed.Checked;
                if (b1)
                {
                    trainState.Speed = Convert.ToDouble(nudSpeed.Value) / 3.6;
                }
                else
                    nudSpeed.Value = Convert.ToDecimal((trainState.Speed * 3.6).ToString("f3"));
                if (b2)
                {
                    trainState.AccSpeed = Convert.ToDouble(nudAccSpeed.Value);
                }
                else
                    nudAccSpeed.Value = Convert.ToDecimal(trainState.AccSpeed.ToString("f3"));
            }
            #endregion

            //EB状态
            if (trainState.EBStatus)
            {
                this.lblEBStatus.Text = "是";
                this.lblEBStatus.ForeColor = Color.Red;
            }
            else
            {
                this.lblEBStatus.Text = "否";
                this.lblEBStatus.ForeColor = Color.Black;
            }
            //制动状态
            if (trainState.BrakeStatus)
            {
                this.lblBrakeStatus.Text = "是";
                this.lblBrakeStatus.ForeColor = Color.Red;
            }
            else
            {
                this.lblBrakeStatus.Text = "否";
                this.lblBrakeStatus.ForeColor = Color.Black;
            }
            //列车完整性
            trainState.TrainIntegrity = this.chkBoxTrainIntegrity.Checked;
            //列车位置
            this.lblTrainLeftLoc.Text = TrainLocation.LocToString(trainState.TrainLocation.LeftLoc);
            this.lblTrainRightLoc.Text = TrainLocation.LocToString(trainState.TrainLocation.RightLoc);
            Balise balise = trainState.TrainLocation.Lrbg;
            this.lblTrainLrbg.Text =balise==null?"0\r\n0":balise .BaliseName + "\r\n" + balise.BaliseNumber+"\r\n"+balise.Nid_lrbg;

            UpdatePureText();
        }
        public void UpdatePureText()
        {
            //纯文本消息
            while (!General_MH.PureText.IsEmpty())
                tbPureText.Text += General_MH.PureText.Pop().ToString() + "\r\n";
            // update TextInfo ,too lazy to write a new function
            string text = TextInfo.GetAllText();
            if(text.Length> tbTextInfo.Text.Length || 
                (! text.Equals(tbTextInfo.Text)))
            {
                tbTextInfo.Text = text;
                tbTextInfo.SelectionStart = text.Length;
                tbTextInfo.ScrollToCaret();
            }
        }
        public void UpdateVehicleInterfaceFun()
        {
            updateVehicleInterface += UpdateVehicleInterface;
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                this.BeginInvoke(updateVehicleInterface);
                Thread.Sleep(100);
            }

        }

        #endregion


        public void StartComm()
        {
            string fileName = System.IO.Directory.GetCurrentDirectory() + "\\CommConfig.ini";
            Communication.Init(fileName);
            updateListView += UpdateListView;
        }

        #region 发包模块
        /// <summary>
        /// 所有发送报文都要使用这个方法
        /// （除了与ISDNIF接口的建立连接和释放连接过程）
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="commType"></param>
        public void SendMsg(AbstractSendMessage asm,_CommType commType)
        {
            if (FaultInjection.IsNoMessageSent) return;

            byte[] sendData = asm.Resolve();
            if(commType == _CommType.RBC)
            {
                sendData = XmlParser.SendData(sendData);
            }
            try
            {
                Communication.SendMsg(sendData, commType);
            }
            catch (IOException) { }
            
            ListViewContent lvc = new ListViewContent(DateTime.Now, asm.NID_MESSAGE, commType, asm);
            this.BeginInvoke(updateListView, lvSendMsg, sendMsgQueue, lvc);
        }
        #endregion

        #region 收包模块    
        private Thread fromNRBCThread, fromRBCThread;
        private MessageHandler rbcMsgHandler = new MessageHandler( _CommType.RBC);
        private MessageHandler nrbcMsgHandler = new MessageHandler(_CommType.NRBC);
        public void StartRecvMsgModule(_CommType commType)
        {
            if (commType == _CommType.NRBC && fromNRBCThread == null)
            {
                nrbcMsgHandler.Init(this);
                fromNRBCThread = new Thread(RecvFromNRBC);
                fromNRBCThread.IsBackground = true;
                fromNRBCThread.Start();
            }
            
            if (commType == _CommType.RBC && fromRBCThread == null)
            {
                rbcMsgHandler.Init(this);
                fromRBCThread = new Thread(RecvFromRBC);
                fromRBCThread.IsBackground = true;
                fromRBCThread.Start();
            }
        }
        public void RecvFromNRBC()
        {
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                try
                {
                    byte[] recvData = Communication.RecvMsg(_CommType.NRBC);
                    if (recvData == null || recvData.Length == 0) continue;
                    AbstractRecvMessage arm = AbstractRecvMessage.GetMessage(recvData);
                    isNRBCConnected = arm.GetMessageID() != 39;
                    nrbcMsgHandler.Handling(arm);

                    ListViewContent lvc = new ListViewContent(DateTime.Now, arm.NID_MESSAGE, _CommType.NRBC, arm);
                    this.BeginInvoke(updateListView, lvRecvMsg, recvMsgQueue, lvc);
                }
                catch (SocketException se)
                { }
                catch (IOException ioe)
                { }
                catch(Exception ex)
                {
                    DebugInfo.WriteToFile(ex.ToString(), "RecvFromNRBC");
                }
            }
        }

        public void RecvFromRBC()
        {
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                try
                {
                    byte[] recvData = Communication.RecvMsg(_CommType.RBC);
                    if (recvData == null) continue;
                    if (isISDNIFConnected)
                    {
                        isISDNIFConnected = !XmlParser.IsDisconnectRecved(recvData);
                        if (!isISDNIFConnected) // received disconnect indication;
                        {
                            isRBCConnected = false;
                            Utilities.TextInfo.Add("received disconnect indication from ISDNIF.");
                            continue;
                        }
                        recvData = XmlParser.RecvData(recvData);
                    }
                    else
                    {
                        int rbcid;
                        XmlParser.ConnAck(recvData, out rbcid);
                        isISDNIFConnected = true;
                        continue;
                    }
                    AbstractRecvMessage arm = AbstractRecvMessage.GetMessage(recvData);
                   
                    isRBCConnected = arm.GetMessageID() != 39;
                    if (!isRBCConnected) isISDNIFConnected = false;
                    rbcMsgHandler.Handling(arm);

                    ListViewContent lvc = new ListViewContent(DateTime.Now, arm.NID_MESSAGE, _CommType.RBC, arm);
                    this.BeginInvoke(updateListView, lvRecvMsg, recvMsgQueue, lvc);
                }
                catch (IOException ioe) { }
                catch (Exception e)
                {
                    if (!(e is NullReferenceException))
                        DebugInfo.WriteToFile(e.ToString(), "RBCRecvEx");
                }
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            int index = listView.SelectedItems[0].Index;
            
            if(sender == lvRecvMsg)
            {
                AbstractRecvMessage arm = recvMsgQueue.IndexOf(index).Arm;
                new ShowMsgForm("Message"+arm.GetMessageID(),arm.ToString());
            }
            if (sender == lvSendMsg)
            {
                AbstractSendMessage asm = sendMsgQueue.IndexOf(index).Asm;
                new ShowMsgForm("Message" + asm.GetMessageID(), asm.ToString());
            }
        }

        private delegate void UpdateListViewEventHandler(ListView lv, CircularQueue<ListViewContent> q, ListViewContent lvc);
        private event UpdateListViewEventHandler updateListView; 
        private void UpdateListView(ListView listView, CircularQueue<ListViewContent> queue,ListViewContent lvc)
        {
            if (lvc == null) return;
            listView.BeginUpdate();
            if (queue.IsFull() == false)
            {
                queue.Push(lvc);
                listView.Items.Add(GetItem(lvc));
            }
            else
            {
                listView.Items.Clear();
                queue.Pop();
                queue.Push(lvc);
                queue.DecreaseToHalf();     //如果消息队列满，则只保留最近的一半消息
                foreach(ListViewContent v in queue)
                {
                    listView.Items.Add(GetItem(v));
                }
            }
            int count = listView.Items.Count;
            int lastIndex = count > 1 ? count - 1 : 0;
            listView.Items[lastIndex].EnsureVisible(); //make scroll bar at the end.
            listView.EndUpdate();
            return; //不加这个return的话，方法会连续执行两次，不知道原因是什么
        }
        private ListViewItem GetItem(ListViewContent lvc)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = lvc.Time.ToString();
            lvi.SubItems.Add(lvc.MsgId.ToString());
            lvi.SubItems.Add(lvc.CommType.ToString());
            return lvi;
        }
        #endregion

        #region 通信连接状态显示
        public void SetConnStatusLabel(ToolStripStatusLabel tssl, bool isConnect, ref DateTime dt)
        {
            try
            {
                DateTime After = DateTime.Now;
                if (dt == null || dt == DateTime.MinValue)
                {
                    dt = After;
                }
                if (!isConnect)
                {
                    if (After.Subtract(dt).TotalMilliseconds > 2000)
                    {
                        Invoke(new Action(() =>
                        {
                            if (tssl != null && !tssl.IsDisposed)//
                            {
                                tssl.BackColor = Color.Red;
                                tssl.Text = "通信断开";
                            }
                        }));
                    }
                    else if (After.Subtract(dt).TotalMilliseconds > 1000)
                    {
                        Invoke(new Action(() =>
                        {
                            if (tssl != null && !tssl.IsDisposed)
                            {
                                tssl.BackColor = Color.Yellow;
                                tssl.Text = "正在连接...";
                            }
                        }));
                    }
                }
                else
                {
                    dt = After;
                    Invoke(new Action(() =>
                    {
                        if (tssl != null && !tssl.IsDisposed)
                        {
                            tssl.BackColor = Color.Green;
                            tssl.Text = "通信连接";
                        }
                    }));
                }
                //    Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        DateTime Before_NRBCConn;
        private bool ConnStatus_NRBC
        {
            get
            {
                return toolStripStatusLabel_NRBC.Text == "通信连接";
            }
            set
            {
                SetConnStatusLabel(toolStripStatusLabel_NRBC, value, ref Before_NRBCConn);
            }
        }

        DateTime Before_RBCConn;
        private bool ConnStatus_RBC
        {
            get
            {
                return toolStripStatusLabel_RBC.Text == "通信连接";
            }
            set
            {
                SetConnStatusLabel(toolStripStatusLabel_RBC, value, ref Before_RBCConn);
            }
        }

        DateTime Before_ISDNIFConn;
        private bool ConnStatus_ISDNIF
        {
            get { return toolStripStatusLabel_ISDNIF.Text == "通信连接"; }
            set { SetConnStatusLabel(toolStripStatusLabel_ISDNIF, value, ref Before_ISDNIFConn); }
        }

        private void timerConnStatus_Tick(object sender, EventArgs e)
        {
            DateTime After = DateTime.Now;
            if (After > Before_NRBCConn)
            {
                if (After.Subtract(Before_NRBCConn).TotalMilliseconds > 1000)
                    ConnStatus_NRBC = Communication.IsConnected(_CommType.NRBC);
            }
            if (After > Before_RBCConn)
            {
                if (After.Subtract(Before_RBCConn).TotalMilliseconds > 1000)
                    ConnStatus_RBC = Communication.IsConnected(_CommType.RBC) && isRBCConnected;
            }
            if (After > Before_ISDNIFConn)
            {
                if (After.Subtract(Before_ISDNIFConn).TotalMilliseconds > 1000)
                    ConnStatus_ISDNIF = Communication.IsConnected(_CommType.RBC) && isISDNIFConnected;
            }
        }
        #endregion

    }
}
