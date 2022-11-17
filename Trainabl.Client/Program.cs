using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor.Services;
using Trainabl.Client;
using Trainabl.Client.Shared;
using Trainabl.Client.Authentication;
using Trainabl.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient("ServerAPI",
                               client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
       .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                                     .CreateClient("ServerAPI"));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices();
builder.Services.AddScoped<UserSettingsService>();

// see https://github.com/dotnet/aspnetcore/issues/40046
builder.Services.AddAuth0OidcAuthentication(options =>
{
	builder.Configuration.Bind("Auth0", options.ProviderOptions);
	options.ProviderOptions.ResponseType = "code";
	options.ProviderOptions.AdditionalProviderParameters.Add(
		"audience", builder.Configuration["Auth0:Audience"]);
	options.ProviderOptions.DefaultScopes.Add("email");
	var authority = builder.Configuration["Auth0:Authority"];
	var clientId  = builder.Configuration["Auth0:ClientId"];
	options.ProviderOptions.MetadataSeed.EndSessionEndpoint =
		$"{authority}/v2/logout?client_id={clientId}&returnTo={builder.HostEnvironment.BaseAddress}";
}).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>();
       

await builder.Build().RunAsync();