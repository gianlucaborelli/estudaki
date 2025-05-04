namespace ProvaOnline.Models
{
    public class SupportImage
    {
        /// <summary>
        /// Tipo de arquivo da imagem.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// String Base64 da imagem.
        /// </summary>
        public string Base64 { get; set; }
    }
}
