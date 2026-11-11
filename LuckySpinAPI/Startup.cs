using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using LuckySpinAPI.Data;
using LuckySpinAPI.Services;

namespace LuckySpinAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // 1. Включаем поддержку контроллеров
            services.AddControllers();

            // 2. Подключаем базу данных в оперативной памяти (In-Memory DB)
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("LuckySpinDb"));

            // 3. Игровая логика: источник случайности, движок правил и сервис ставок
            services.AddSingleton<IRandomSource, SystemRandomSource>();
            services.AddSingleton<SlotEngine>();
            services.AddScoped<SlotService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
