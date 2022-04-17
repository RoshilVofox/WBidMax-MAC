using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.PortableLibrary
{
    public static class ProtoSerailizer
    {
        public static void SerializeObject<T>(string fileNameAndPath, T objectToSerialize,Stream filestream)
        {
            try
            {

                ProtoBuf.Serializer.Serialize(filestream, objectToSerialize);
            }
            catch (Exception ex)
            {
                
                throw;
            }
             
        }
        public static T DeSerializeObject<T>(string fileNameAndPath, T entity, Stream filestream)
        {
            try
            {
                entity = ProtoBuf.Serializer.Deserialize<T>(filestream);
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entity;
        }

        //public static LineInfo DeSerializeObject1(string fileNameAndPath, LineInfo entity, Stream filestream)
        //{
        //    try
        //    {

        //        entity = ProtoBuf.Serializer.Deserialize<LineInfo>(filestream);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return entity;
        //}
    }
}
