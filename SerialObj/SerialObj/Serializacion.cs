using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;


namespace SerialObj
{
    public class Serializacion
    {
        /// <summary>
        /// Guarda el Objeto (serializable) en la ruta especificada.
        /// </summary>
        /// <param name="Patch">Ruta completa donde se guardará el objeto</param>
        /// <param name="Objeto"></param>
        public void GuardaObjeto(string Patch, object Objeto)
        {
            //serializa y escribe en archivo pasando por MemorySteam
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, Objeto);

                //comprimimos y escribimos
                MemoryStream comprimido = Compress(ms, CompressionLevel.Optimal);

                //lo guardamos en archivo
                using (FileStream salida = File.OpenWrite(Patch))
                {
                    comprimido.CopyTo(salida);
                    salida.Flush();
                    salida.Close();
                }
            }
        }

        /// <summary>
        /// Carga el objeto de la ruta especificada.
        /// </summary>
        /// <param name="Patch">Ruta completa del objeto a cargar</param>
        /// <returns>Retorna el objeto cargado</returns>
        public object CargaObjeto(string Patch)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream lectura = File.OpenRead(Patch))
                {
                    lectura.CopyTo(ms);
                    lectura.Flush();
                    lectura.Close();
                }

                MemoryStream descomprimido = Decompress(ms);

                var binfor = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                descomprimido.Position = 0;
                return binfor.Deserialize(descomprimido);
            }
        }


        private MemoryStream Compress(Stream decompressed, CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            decompressed.Position = 0;
            var compressed = new MemoryStream();
            using (var zip = new GZipStream(compressed, compressionLevel, true))
            {
                decompressed.CopyTo(zip);
            }

            compressed.Seek(0, SeekOrigin.Begin);
            return compressed;
        }


        private MemoryStream Decompress(Stream compressed)
        {
            compressed.Position = 0;
            var decompressed = new MemoryStream();
            using (var zip = new GZipStream(compressed, CompressionMode.Decompress, true))
            {
                zip.CopyTo(decompressed);
            }

            decompressed.Seek(0, SeekOrigin.Begin);
            return decompressed;
        }

    }
}
