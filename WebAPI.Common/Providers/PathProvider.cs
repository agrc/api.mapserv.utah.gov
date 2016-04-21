using System.Web.Hosting;

namespace WebAPI.Common.Providers
{
    public class PathProvider
    {
        /// <summary>
        ///     "~/App_Data/CityZipLookup.txt"
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public virtual string GetPathTo(string file)
        {
            return HostingEnvironment.MapPath(file);
        }
    }
}