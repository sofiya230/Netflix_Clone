using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetflixClone.Interfaces;
using NetflixClone.Services;

namespace NetflixClone.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IContentService, ContentService>();
            services.AddScoped<IEpisodeService, EpisodeService>();
            services.AddScoped<IWatchHistoryService, WatchHistoryService>();
            services.AddScoped<IMyListService, MyListService>();
            
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            services.AddScoped<IEmailService, GmailEmailService>();
            services.AddScoped<ICodeGenerationService, CodeGenerationService>();
            services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();

            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
