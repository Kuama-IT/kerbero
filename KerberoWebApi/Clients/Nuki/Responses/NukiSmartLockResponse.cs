using KerberoWebApi.Clients.Responses;

namespace KerberoWebApi.Clients.Nuki.Responses;

public class NukiSmartLockResponse: ISmartLockResponse
{
    public int smartlockId { get; set; }
    public int accountId { get; set; }
    public int type { get; set; }
    public int lmType { get; set; }
    public int authId { get; set; }
    public string name { get; set; } = null!;
    public ISmartLockResponse.State state { get; set; }
    public bool favorite { get; set; }
    public Config config { get; set; } = null!;
    public AdvancedConfig advancedConfig { get; set; } = null!;
    public OpenerAdvancedConfig openerAdvancedConfig { get; set; } = null!;
    public SmartdoorAdvancedConfig smartdoorAdvancedConfig { get; set; } = null!;
    public WebConfig webConfig { get; set; } = null!;
    public int firmwareVersion { get; set; }
    public int hardwareVersion { get; set; }
    public string operationId { get; set; } = null!;
    public int serverState { get; set; }
    public int adminPinState { get; set; }
    public bool virtualDevice { get; set; }
    public DateTime creationDate { get; set; }
    public DateTime updateDate { get; set; }
    public List<Subscription> subscriptions { get; set; } = null!;
    public bool opener { get; set; }
    public bool box { get; set; }
    public bool smartDoor { get; set; }
    public bool keyturner { get; set; }
    public bool smartlock3 { get; set; }
    public class AdvancedConfig
    {
        public int lngTimeout { get; set; }
        public int singleButtonPressAction { get; set; }
        public int doubleButtonPressAction { get; set; }
        public bool automaticBatteryTypeDetection { get; set; }
        public int unlatchDuration { get; set; }
        public string operationId { get; set; } = null!;
        public int totalDegrees { get; set; }
        public int singleLockedPositionOffsetDegrees { get; set; }
        public int unlockedToLockedTransitionOffsetDegrees { get; set; }
        public int unlockedPositionOffsetDegrees { get; set; }
        public int lockedPositionOffsetDegrees { get; set; }
        public bool detachedCylinder { get; set; }
        public int batteryType { get; set; }
        public bool autoLock { get; set; }
        public int autoLockTimeout { get; set; }
        public bool autoUpdateEnabled { get; set; }
    }

    public class Config
    {
        public string name { get; set; } = null!;
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int capabilities { get; set; }
        public bool autoUnlatch { get; set; }
        public bool liftUpHandle { get; set; }
        public bool pairingEnabled { get; set; }
        public bool buttonEnabled { get; set; }
        public bool ledEnabled { get; set; }
        public int ledBrightness { get; set; }
        public int timezoneOffset { get; set; }
        public int daylightSavingMode { get; set; }
        public bool fobPaired { get; set; }
        public int fobAction1 { get; set; }
        public int fobAction2 { get; set; }
        public int fobAction3 { get; set; }
        public bool singleLock { get; set; }
        public int operatingMode { get; set; }
        public int advertisingMode { get; set; }
        public bool keypadPaired { get; set; }
        public int homekitState { get; set; }
        public int timezoneId { get; set; }
        public int deviceType { get; set; }
        public bool wifiEnabled { get; set; }
        public string operationId { get; set; } = null!;
    }

    public class OpenerAdvancedConfig
    {
        public int intercomId { get; set; }
        public int busModeSwitch { get; set; }
        public int shortCircuitDuration { get; set; }
        public int electricStrikeDelay { get; set; }
        public bool randomElectricStrikeDelay { get; set; }
        public int electricStrikeDuration { get; set; }
        public bool disableRtoAfterRing { get; set; }
        public int rtoTimeout { get; set; }
        public int doorbellSuppression { get; set; }
        public int doorbellSuppressionDuration { get; set; }
        public int soundRing { get; set; }
        public int soundOpen { get; set; }
        public int soundRto { get; set; }
        public int soundCm { get; set; }
        public int soundConfirmation { get; set; }
        public int soundLevel { get; set; }
        public int singleButtonPressAction { get; set; }
        public int doubleButtonPressAction { get; set; }
        public int batteryType { get; set; }
        public bool automaticBatteryTypeDetection { get; set; }
        public bool autoUpdateEnabled { get; set; }
        public string operationId { get; set; } = null!;
    }

    public class SmartdoorAdvancedConfig
    {
        public int lngTimeout { get; set; }
        public int singleButtonPressAction { get; set; }
        public int doubleButtonPressAction { get; set; }
        public bool automaticBatteryTypeDetection { get; set; }
        public int unlatchDuration { get; set; }
        public string operationId { get; set; } = null!;
        public int buzzerVolume { get; set; }
        public List<int> supportedBatteryTypes { get; set; } = null!;
        public int batteryType { get; set; }
        public int autoLockTimeout { get; set; }
        public bool autoLock { get; set; }
    }

    public class Subscription
    {
        public string type { get; set; } = null!;
        public string state { get; set; } = null!;
        public DateTime updateDate { get; set; }
        public DateTime creationDate { get; set; }
    }

    public class WebConfig
    {
        public bool batteryWarningPerMailEnabled { get; set; }
        public List<int> dismissedLiftUpHandleWarning { get; set; } = null!;
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