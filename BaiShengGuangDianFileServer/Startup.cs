using BaiShengGuangDianFileServer.Base.FileConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModelBase.Base.Filter;
using ModelBase.Base.Logger;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace BaiShengGuangDianFileServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    //忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    ////不使用驼峰样式的key
                    //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    //设置时间格式
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });


            //注册过滤器
            services.AddMvc(options =>
            {
                options.Filters.Add<HttpGlobalExceptionFilter>();
            });

            //services.AddSwaggerGen(c =>
            //{
            //    //配置第一个Doc
            //    c.SwaggerDoc("v1", new Info { Title = "API_List", Version = "v1" });
            //});
            //跨域
            services.AddCors(options => options.AddPolicy("default",
                policy => policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().AllowCredentials()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseCors("default");

            //添加MIME
            var provider = new FileExtensionContentTypeProvider
            {
                Mappings =
                {
                    [".bin"] = "application/octet-stream",
                    [".npc"] = "application/octet-stream",
                }
            };
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            //// Enable the Swagger UI middleware and the Swagger generator
            ////部署时将其注释掉或者启用授权
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_List");
            //    c.RoutePrefix = "swagger";
            //    c.DocExpansion(DocExpansion.None);
            //});

            //app.UseSwagger();
            app.UseMvcWithDefaultRoute();
            FilePath.RootPath = env.WebRootPath;
            Log.Info("Server Start");
        }
    }
}
