namespace KerberoWebApi.Clients.Responses;

public interface ISmartLockResponse
{
    public int smartlockId { get; set; }
    public string name { get; set; }
    public State state { get; set; }
    public class State
    {
        public int mode { get; set; }
        public int state { get; set; }
        public int trigger { get; set; }
        public int lastAction { get; set; }
        public bool batteryCritical { get; set; }
        public bool batteryCharging { get; set; }
        public int batteryCharge { get; set; }
        public bool keypadBatteryCritical { get; set; }
        public bool doorsensorBatteryCritical { get; set; }
        public int doorState { get; set; }
        public int ringToOpenTimer { get; set; }
        public DateTime ringToOpenEnd { get; set; }
        public bool nightMode { get; set; }
        public string operationId { get; set; } = null!;
    }
        
    public enum SmartLockState
    {
        Uncalibrated = 0,
        Locked = 1,
        Unlocking = 2,
        Unlocked = 3, 
        Locking = 4,
        Unlatched = 5,
        LockNGo = 6, 
        Unlatching = 7,
        MotorBlocked = 254,
        Undefined = 255
    }

    public enum LastAction
    {
        System, 
        Manual,
        Button,
        Automatic,
        Web,
        App,
        ContinuousMode,
        Accessory
    }
}