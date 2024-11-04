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
            // Isso evita a substituição de arquivos com o mesmo nome
            string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);

            // Define o caminho absoluto onde a imagem será salva no servidor
            // Usa Server.MapPath para converter o diretório de upload para o caminho físico do servidor
            string imagePath = Path.Combine(HttpContext.Current.Server.MapPath(uploadDir), fileName);

            // Define o caminho relativo da imagem, que será retornado e salvo no banco de dados
            // Esse caminho é usado posteriormente para exibir a imagem no sistema
            string imageUrl = Path.Combine(uploadDir, fileName);

            // Salva o arquivo da imagem no servidor no caminho absoluto especificado
            imageFile.SaveAs(imagePath);

            // Retorna o caminho relativo da imagem para que possa ser armazenado no banco de dados
            // Isso permite que o caminho seja usado para exibir a imagem na aplicação
            return imageUrl;
        }
    }
}