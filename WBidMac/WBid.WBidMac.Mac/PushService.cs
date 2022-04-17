//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by SlSvcUtil, version 5.0.61118.0
// 
namespace WBidPushService.Model
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PushDeviceDetails", Namespace="http://schemas.datacontract.org/2004/07/WBidPushService.Model")]
    public partial class PushDeviceDetails : object
    {
        
        private int BadgeCountField;
        
        private System.Guid DeviceIdField;
        
        private string DeviceTockenField;
        
        private string DeviceTypeField;
        
        private int EmpNoField;
        
        private bool IsActiveField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BadgeCount
        {
            get
            {
                return this.BadgeCountField;
            }
            set
            {
                this.BadgeCountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid DeviceId
        {
            get
            {
                return this.DeviceIdField;
            }
            set
            {
                this.DeviceIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DeviceTocken
        {
            get
            {
                return this.DeviceTockenField;
            }
            set
            {
                this.DeviceTockenField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DeviceType
        {
            get
            {
                return this.DeviceTypeField;
            }
            set
            {
                this.DeviceTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int EmpNo
        {
            get
            {
                return this.EmpNoField;
            }
            set
            {
                this.EmpNoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return this.IsActiveField;
            }
            set
            {
                this.IsActiveField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IWBidPushSerivce")]
public interface IWBidPushSerivce
{
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IWBidPushSerivce/RegistorDevices", ReplyAction="http://tempuri.org/IWBidPushSerivce/RegistorDevicesResponse")]
    System.IAsyncResult BeginRegistorDevices(WBidPushService.Model.PushDeviceDetails pushDeviceDetails, System.AsyncCallback callback, object asyncState);
    
    int EndRegistorDevices(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IWBidPushSerivce/ResetBadge", ReplyAction="http://tempuri.org/IWBidPushSerivce/ResetBadgeResponse")]
    System.IAsyncResult BeginResetBadge(System.Guid deviceId, System.AsyncCallback callback, object asyncState);
    
    int EndResetBadge(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IWBidPushSerivce/PushMessage")]
    System.IAsyncResult BeginPushMessage(long pushmessageId, System.AsyncCallback callback, object asyncState);
    
    void EndPushMessage(System.IAsyncResult result);
    
    [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IWBidPushSerivce/PushMessageManually")]
    System.IAsyncResult BeginPushMessageManually(string message, int position, int device, System.AsyncCallback callback, object asyncState);
    
    void EndPushMessageManually(System.IAsyncResult result);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IWBidPushSerivceChannel : IWBidPushSerivce, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class RegistorDevicesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{
    
    private object[] results;
    
    public RegistorDevicesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState)
    {
        this.results = results;
    }
    
    public int Result
    {
        get
        {
            base.RaiseExceptionIfNecessary();
            return ((int)(this.results[0]));
        }
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class ResetBadgeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
{
    
    private object[] results;
    
    public ResetBadgeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState)
    {
        this.results = results;
    }
    
    public int Result
    {
        get
        {
            base.RaiseExceptionIfNecessary();
            return ((int)(this.results[0]));
        }
    }
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class WBidPushSerivceClient : System.ServiceModel.ClientBase<IWBidPushSerivce>, IWBidPushSerivce
{
    
    private BeginOperationDelegate onBeginRegistorDevicesDelegate;
    
    private EndOperationDelegate onEndRegistorDevicesDelegate;
    
    private System.Threading.SendOrPostCallback onRegistorDevicesCompletedDelegate;
    
    private BeginOperationDelegate onBeginResetBadgeDelegate;
    
    private EndOperationDelegate onEndResetBadgeDelegate;
    
    private System.Threading.SendOrPostCallback onResetBadgeCompletedDelegate;
    
    private BeginOperationDelegate onBeginPushMessageDelegate;
    
    private EndOperationDelegate onEndPushMessageDelegate;
    
    private System.Threading.SendOrPostCallback onPushMessageCompletedDelegate;
    
    private BeginOperationDelegate onBeginPushMessageManuallyDelegate;
    
    private EndOperationDelegate onEndPushMessageManuallyDelegate;
    
    private System.Threading.SendOrPostCallback onPushMessageManuallyCompletedDelegate;
    
    private BeginOperationDelegate onBeginOpenDelegate;
    
    private EndOperationDelegate onEndOpenDelegate;
    
    private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
    
    private BeginOperationDelegate onBeginCloseDelegate;
    
    private EndOperationDelegate onEndCloseDelegate;
    
    private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
    
    public WBidPushSerivceClient()
    {
    }
    
    public WBidPushSerivceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public WBidPushSerivceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public WBidPushSerivceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public WBidPushSerivceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public System.Net.CookieContainer CookieContainer
    {
        get
        {
            System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
            if ((httpCookieContainerManager != null))
            {
                return httpCookieContainerManager.CookieContainer;
            }
            else
            {
                return null;
            }
        }
        set
        {
            System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
            if ((httpCookieContainerManager != null))
            {
                httpCookieContainerManager.CookieContainer = value;
            }
            else
            {
                throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                        "ookieContainerBindingElement.");
            }
        }
    }
    
    public event System.EventHandler<RegistorDevicesCompletedEventArgs> RegistorDevicesCompleted;
    
    public event System.EventHandler<ResetBadgeCompletedEventArgs> ResetBadgeCompleted;
    
    public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> PushMessageCompleted;
    
    public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> PushMessageManuallyCompleted;
    
    public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
    
    public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    System.IAsyncResult IWBidPushSerivce.BeginRegistorDevices(WBidPushService.Model.PushDeviceDetails pushDeviceDetails, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginRegistorDevices(pushDeviceDetails, callback, asyncState);
    }
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    int IWBidPushSerivce.EndRegistorDevices(System.IAsyncResult result)
    {
        return base.Channel.EndRegistorDevices(result);
    }
    
    private System.IAsyncResult OnBeginRegistorDevices(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        WBidPushService.Model.PushDeviceDetails pushDeviceDetails = ((WBidPushService.Model.PushDeviceDetails)(inValues[0]));
        return ((IWBidPushSerivce)(this)).BeginRegistorDevices(pushDeviceDetails, callback, asyncState);
    }
    
    private object[] OnEndRegistorDevices(System.IAsyncResult result)
    {
        int retVal = ((IWBidPushSerivce)(this)).EndRegistorDevices(result);
        return new object[] {
                retVal};
    }
    
    private void OnRegistorDevicesCompleted(object state)
    {
        if ((this.RegistorDevicesCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.RegistorDevicesCompleted(this, new RegistorDevicesCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void RegistorDevicesAsync(WBidPushService.Model.PushDeviceDetails pushDeviceDetails)
    {
        this.RegistorDevicesAsync(pushDeviceDetails, null);
    }
    
    public void RegistorDevicesAsync(WBidPushService.Model.PushDeviceDetails pushDeviceDetails, object userState)
    {
        if ((this.onBeginRegistorDevicesDelegate == null))
        {
            this.onBeginRegistorDevicesDelegate = new BeginOperationDelegate(this.OnBeginRegistorDevices);
        }
        if ((this.onEndRegistorDevicesDelegate == null))
        {
            this.onEndRegistorDevicesDelegate = new EndOperationDelegate(this.OnEndRegistorDevices);
        }
        if ((this.onRegistorDevicesCompletedDelegate == null))
        {
            this.onRegistorDevicesCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRegistorDevicesCompleted);
        }
        base.InvokeAsync(this.onBeginRegistorDevicesDelegate, new object[] {
                    pushDeviceDetails}, this.onEndRegistorDevicesDelegate, this.onRegistorDevicesCompletedDelegate, userState);
    }
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    System.IAsyncResult IWBidPushSerivce.BeginResetBadge(System.Guid deviceId, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginResetBadge(deviceId, callback, asyncState);
    }
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    int IWBidPushSerivce.EndResetBadge(System.IAsyncResult result)
    {
        return base.Channel.EndResetBadge(result);
    }
    
    private System.IAsyncResult OnBeginResetBadge(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        System.Guid deviceId = ((System.Guid)(inValues[0]));
        return ((IWBidPushSerivce)(this)).BeginResetBadge(deviceId, callback, asyncState);
    }
    
    private object[] OnEndResetBadge(System.IAsyncResult result)
    {
        int retVal = ((IWBidPushSerivce)(this)).EndResetBadge(result);
        return new object[] {
                retVal};
    }
    
    private void OnResetBadgeCompleted(object state)
    {
        if ((this.ResetBadgeCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.ResetBadgeCompleted(this, new ResetBadgeCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void ResetBadgeAsync(System.Guid deviceId)
    {
        this.ResetBadgeAsync(deviceId, null);
    }
    
    public void ResetBadgeAsync(System.Guid deviceId, object userState)
    {
        if ((this.onBeginResetBadgeDelegate == null))
        {
            this.onBeginResetBadgeDelegate = new BeginOperationDelegate(this.OnBeginResetBadge);
        }
        if ((this.onEndResetBadgeDelegate == null))
        {
            this.onEndResetBadgeDelegate = new EndOperationDelegate(this.OnEndResetBadge);
        }
        if ((this.onResetBadgeCompletedDelegate == null))
        {
            this.onResetBadgeCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnResetBadgeCompleted);
        }
        base.InvokeAsync(this.onBeginResetBadgeDelegate, new object[] {
                    deviceId}, this.onEndResetBadgeDelegate, this.onResetBadgeCompletedDelegate, userState);
    }
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    System.IAsyncResult IWBidPushSerivce.BeginPushMessage(long pushmessageId, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginPushMessage(pushmessageId, callback, asyncState);
    }
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    void IWBidPushSerivce.EndPushMessage(System.IAsyncResult result)
    {
        base.Channel.EndPushMessage(result);
    }
    
    private System.IAsyncResult OnBeginPushMessage(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        long pushmessageId = ((long)(inValues[0]));
        return ((IWBidPushSerivce)(this)).BeginPushMessage(pushmessageId, callback, asyncState);
    }
    
    private object[] OnEndPushMessage(System.IAsyncResult result)
    {
        ((IWBidPushSerivce)(this)).EndPushMessage(result);
        return null;
    }
    
    private void OnPushMessageCompleted(object state)
    {
        if ((this.PushMessageCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.PushMessageCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void PushMessageAsync(long pushmessageId)
    {
        this.PushMessageAsync(pushmessageId, null);
    }
    
    public void PushMessageAsync(long pushmessageId, object userState)
    {
        if ((this.onBeginPushMessageDelegate == null))
        {
            this.onBeginPushMessageDelegate = new BeginOperationDelegate(this.OnBeginPushMessage);
        }
        if ((this.onEndPushMessageDelegate == null))
        {
            this.onEndPushMessageDelegate = new EndOperationDelegate(this.OnEndPushMessage);
        }
        if ((this.onPushMessageCompletedDelegate == null))
        {
            this.onPushMessageCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnPushMessageCompleted);
        }
        base.InvokeAsync(this.onBeginPushMessageDelegate, new object[] {
                    pushmessageId}, this.onEndPushMessageDelegate, this.onPushMessageCompletedDelegate, userState);
    }
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    System.IAsyncResult IWBidPushSerivce.BeginPushMessageManually(string message, int position, int device, System.AsyncCallback callback, object asyncState)
    {
        return base.Channel.BeginPushMessageManually(message, position, device, callback, asyncState);
    }
    
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    void IWBidPushSerivce.EndPushMessageManually(System.IAsyncResult result)
    {
        base.Channel.EndPushMessageManually(result);
    }
    
    private System.IAsyncResult OnBeginPushMessageManually(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        string message = ((string)(inValues[0]));
        int position = ((int)(inValues[1]));
        int device = ((int)(inValues[2]));
        return ((IWBidPushSerivce)(this)).BeginPushMessageManually(message, position, device, callback, asyncState);
    }
    
    private object[] OnEndPushMessageManually(System.IAsyncResult result)
    {
        ((IWBidPushSerivce)(this)).EndPushMessageManually(result);
        return null;
    }
    
    private void OnPushMessageManuallyCompleted(object state)
    {
        if ((this.PushMessageManuallyCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.PushMessageManuallyCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void PushMessageManuallyAsync(string message, int position, int device)
    {
        this.PushMessageManuallyAsync(message, position, device, null);
    }
    
    public void PushMessageManuallyAsync(string message, int position, int device, object userState)
    {
        if ((this.onBeginPushMessageManuallyDelegate == null))
        {
            this.onBeginPushMessageManuallyDelegate = new BeginOperationDelegate(this.OnBeginPushMessageManually);
        }
        if ((this.onEndPushMessageManuallyDelegate == null))
        {
            this.onEndPushMessageManuallyDelegate = new EndOperationDelegate(this.OnEndPushMessageManually);
        }
        if ((this.onPushMessageManuallyCompletedDelegate == null))
        {
            this.onPushMessageManuallyCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnPushMessageManuallyCompleted);
        }
        base.InvokeAsync(this.onBeginPushMessageManuallyDelegate, new object[] {
                    message,
                    position,
                    device}, this.onEndPushMessageManuallyDelegate, this.onPushMessageManuallyCompletedDelegate, userState);
    }
    
    private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
    }
    
    private object[] OnEndOpen(System.IAsyncResult result)
    {
        ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
        return null;
    }
    
    private void OnOpenCompleted(object state)
    {
        if ((this.OpenCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void OpenAsync()
    {
        this.OpenAsync(null);
    }
    
    public void OpenAsync(object userState)
    {
        if ((this.onBeginOpenDelegate == null))
        {
            this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
        }
        if ((this.onEndOpenDelegate == null))
        {
            this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
        }
        if ((this.onOpenCompletedDelegate == null))
        {
            this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
        }
        base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
    }
    
    private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState)
    {
        return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
    }
    
    private object[] OnEndClose(System.IAsyncResult result)
    {
        ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
        return null;
    }
    
    private void OnCloseCompleted(object state)
    {
        if ((this.CloseCompleted != null))
        {
            InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
            this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }
    }
    
    public void CloseAsync()
    {
        this.CloseAsync(null);
    }
    
    public void CloseAsync(object userState)
    {
        if ((this.onBeginCloseDelegate == null))
        {
            this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
        }
        if ((this.onEndCloseDelegate == null))
        {
            this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
        }
        if ((this.onCloseCompletedDelegate == null))
        {
            this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
        }
        base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
    }
    
    protected override IWBidPushSerivce CreateChannel()
    {
        return new WBidPushSerivceClientChannel(this);
    }
    
    private class WBidPushSerivceClientChannel : ChannelBase<IWBidPushSerivce>, IWBidPushSerivce
    {
        
        public WBidPushSerivceClientChannel(System.ServiceModel.ClientBase<IWBidPushSerivce> client) : 
                base(client)
        {
        }
        
        public System.IAsyncResult BeginRegistorDevices(WBidPushService.Model.PushDeviceDetails pushDeviceDetails, System.AsyncCallback callback, object asyncState)
        {
            object[] _args = new object[1];
            _args[0] = pushDeviceDetails;
            System.IAsyncResult _result = base.BeginInvoke("RegistorDevices", _args, callback, asyncState);
            return _result;
        }
        
        public int EndRegistorDevices(System.IAsyncResult result)
        {
            object[] _args = new object[0];
            int _result = ((int)(base.EndInvoke("RegistorDevices", _args, result)));
            return _result;
        }
        
        public System.IAsyncResult BeginResetBadge(System.Guid deviceId, System.AsyncCallback callback, object asyncState)
        {
            object[] _args = new object[1];
            _args[0] = deviceId;
            System.IAsyncResult _result = base.BeginInvoke("ResetBadge", _args, callback, asyncState);
            return _result;
        }
        
        public int EndResetBadge(System.IAsyncResult result)
        {
            object[] _args = new object[0];
            int _result = ((int)(base.EndInvoke("ResetBadge", _args, result)));
            return _result;
        }
        
        public System.IAsyncResult BeginPushMessage(long pushmessageId, System.AsyncCallback callback, object asyncState)
        {
            object[] _args = new object[1];
            _args[0] = pushmessageId;
            System.IAsyncResult _result = base.BeginInvoke("PushMessage", _args, callback, asyncState);
            return _result;
        }
        
        public void EndPushMessage(System.IAsyncResult result)
        {
            object[] _args = new object[0];
            base.EndInvoke("PushMessage", _args, result);
        }
        
        public System.IAsyncResult BeginPushMessageManually(string message, int position, int device, System.AsyncCallback callback, object asyncState)
        {
            object[] _args = new object[3];
            _args[0] = message;
            _args[1] = position;
            _args[2] = device;
            System.IAsyncResult _result = base.BeginInvoke("PushMessageManually", _args, callback, asyncState);
            return _result;
        }
        
        public void EndPushMessageManually(System.IAsyncResult result)
        {
            object[] _args = new object[0];
            base.EndInvoke("PushMessageManually", _args, result);
        }
    }
}
