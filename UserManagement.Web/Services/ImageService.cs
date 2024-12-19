using System;
using System.IO;
using System.Web;

namespace UserManagement.Web.Services
{
    public class ImageService
    {
        private readonly string uploadDir;

        public ImageService(string uploadDir)
        {
            this.uploadDir = uploadDir;
        }

        public string SaveImage(HttpPostedFileBase imageFile)
        {
            // Verifica se um arquivo de imagem foi enviado e se ele possui conteúdo
            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                return null;
            }

            // Gera um nome de arquivo único usando um GUID, combinado com a extensão do arquivo original
            string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);

            // Define o caminho absoluto onde a imagem será guardado no servidor
            string imagePath = Path.Combine(HttpContext.Current.Server.MapPath(uploadDir), fileName);

            // Define o caminho relativo da imagem, que será retornado e salvo no banco de dados
            string imageUrl = Path.Combine(uploadDir, fileName);

            // Guarda o arquivo da imagem no servidor no caminho absoluto especificado
            imageFile.SaveAs(imagePath);

            return imageUrl;
        }
    }
}