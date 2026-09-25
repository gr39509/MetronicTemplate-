// Hand-written companion to the NSwag-generated ApiClient.cs.
// The generated result wrappers all share Success/Message/Data but no common type;
// these partial declarations give them one so callers can handle results uniformly.
// Safe to keep when regenerating the client: only add a line when a new *ApiResult appears.

#nullable disable
using System.Collections.Generic;

namespace NsawaWeb.Services.Services;

public interface IApiResult<out T>
{
    bool Success { get; }
    string Message { get; }
    T Data { get; }
}

public partial class DonationRequestViewModelApiResult : IApiResult<DonationRequestViewModel> { }
public partial class DonationResponseModelApiResult : IApiResult<DonationResponseModel> { }
public partial class DonationSummaryViewModelApiResult : IApiResult<DonationSummaryViewModel> { }
public partial class DonationViewModelListApiResult : IApiResult<ICollection<DonationViewModel>> { }
public partial class DonorViewModelApiResult : IApiResult<DonorViewModel> { }
public partial class EventAffiliateViewModelListApiResult : IApiResult<ICollection<EventAffiliateViewModel>> { }
public partial class EventGroupViewModelApiResult : IApiResult<EventGroupViewModel> { }
public partial class EventGroupViewModelListApiResult : IApiResult<ICollection<EventGroupViewModel>> { }
public partial class EventTypeViewModelListApiResult : IApiResult<ICollection<EventTypeViewModel>> { }
public partial class EventViewModelApiResult : IApiResult<EventViewModel> { }
public partial class EventViewModelListApiResult : IApiResult<ICollection<EventViewModel>> { }
public partial class LoginResponseModelApiResult : IApiResult<LoginResponseModel> { }
public partial class MoMoNumberVerificationResponseApiResult : IApiResult<MoMoNumberVerificationResponse> { }
public partial class NetworkTypeListApiResult : IApiResult<ICollection<NetworkType>> { }
public partial class PaymentTypeViewModelListApiResult : IApiResult<ICollection<PaymentTypeViewModel>> { }
public partial class RoleViewModelListApiResult : IApiResult<ICollection<RoleViewModel>> { }
public partial class StringApiResult : IApiResult<string> { }
public partial class UserAccountViewModelApiResult : IApiResult<UserAccountViewModel> { }
