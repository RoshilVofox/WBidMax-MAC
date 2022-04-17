#region NameSpace
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace WBid.WBidiPad.SharedLibrary.Serialization
{
    public class Serialization
    {

        //private void SerializeObject(string filename, Object objectToSerialize)
        //{
        //    MemoryStream memStream = new MemoryStream();
        //    BinaryFormatter binaryFormatter = new BinaryFormatter();
        //    binaryFormatter.Serialize(memStream, objectToSerialize);
        //    byte[] byteArray = memStream.ToArray();
        //    FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
        //    fileStream.Write(byteArray.ToArray(), 0, byteArray.Length);
        //    fileStream.Close();
        //    memStream.Close();
        //    memStream.Dispose();
        //}


        /// <summary>
        /// Serialize an object and write to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileNameAndPath"></param>
        /// <param name="objectToSerialize"></param>
        public static void SerializeObject<T>(string fileNameAndPath, T objectToSerialize)
        {
            
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(stream, objectToSerialize);
                string objects = Encoding.ASCII.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                byte[] PinByte = Encoding.ASCII.GetBytes(objects);
                using (FileStream writer = new FileStream(fileNameAndPath, FileMode.OpenOrCreate))
                {
                    writer.Write(PinByte, 0, PinByte.Length);
                    writer.Close();
                }
            }
        }


        /// <summary>
        /// Deserialize object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileNameAndPath"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(string fileNameAndPath, T entity)
        {

            using (FileStream readstream = new FileStream(fileNameAndPath, FileMode.Open, FileAccess.Read))
            {
                using (Stream reader = new StreamReader(readstream).BaseStream)
                {
                    byte[] pinArray = new byte[reader.Length];
                    reader.Read(pinArray, 0, pinArray.Length);

                    string result = Encoding.ASCII.GetString(pinArray, 0, pinArray.Length);
                    if (result.Length > 0)
                        using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(result)))
                        {
                            var ser = new DataContractJsonSerializer(typeof(T));
                            entity = (T)ser.ReadObject(ms);

                        }

                    reader.Close();
                    readstream.Close();
                }
            }
            return entity;
        }




        public static void SSerializeObject<T>(string fileNameAndPath, T objectToSerialize)
        {


            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(stream, objectToSerialize);
                // string objects = Encoding.ASCII.GetString(stream.GetBuffer(), 0, (int)stream.Length);

                // byte[] bytes = new byte[stream.Length];
                // stream.Read(bytes, 0, (int)stream.Length);
                using (FileStream file = new FileStream(fileNameAndPath, FileMode.OpenOrCreate))
                {
                    stream.WriteTo(file);
                    file.Flush();
                    //file.Write(bytes, 0, bytes.Length);
                    file.Close();
                }
            }
        }

        public static T DDeSerializeObject<T>(string fileNameAndPath, T entity)
        {

            using (FileStream readstream = new FileStream(fileNameAndPath, FileMode.Open, FileAccess.Read))
            {
                using (Stream reader = new StreamReader(readstream).BaseStream)
                {
                    byte[] pinArray = new byte[reader.Length];
                    reader.Read(pinArray, 0, pinArray.Length);

                   // string result = Encoding.ASCII.GetString(pinArray, 0, pinArray.Length);
                   // if (result.Length > 0)
                        using (var ms = new MemoryStream(pinArray))
                        {
                            var ser = new DataContractJsonSerializer(typeof(T));
                            entity = (T)ser.ReadObject(ms);

                        }

                    reader.Close();
                    readstream.Close();
                }
            }
            return entity;
        }
    }
}
