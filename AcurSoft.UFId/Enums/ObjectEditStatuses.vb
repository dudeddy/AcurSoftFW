Namespace Enums

    Public Enum ObjectEditStatuses
        JustCreated = 1 << 1
        NewSaved = 2 << 1
        Draft = 3 << 1
        Pending = 4 << 1
        Ready = 10 << 1
        Canceled = 20 << 1
        Deleted = 100 << 1
        Locked = 500 << 1
        Other = 1000 << 1
    End Enum
End Namespace
