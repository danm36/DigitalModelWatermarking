using DMWEditor;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DMWEditor
{
    [ModelInfo("DMW Model", "dmw")]
    public class DMWModelImporter : ModelImporter
    {
        const int DMW_CURRENT_VERSION = 1;
        const int DMW_LOWEST_SUPPORTED_VERSION = 1;

        const byte MARKER_VERT = (byte)'V';
        const byte MARKER_INDEX = (byte)'I';

        //Encryption methods
        enum DMW_DATAENC : short
        {
            None = 0,
            Rijndael = 1,
        }
        static readonly DMW_DATAENC DefaultEncoder = DMW_DATAENC.Rijndael;

        Dictionary<DMW_DATAENC, Action<byte[], string, MemoryStream>> Encryptors = new Dictionary<DMW_DATAENC, Action<byte[], string, MemoryStream>>()
        {
            { DMW_DATAENC.Rijndael, RijndaelEncrypt }
        };

        Dictionary<DMW_DATAENC, Func<byte[], string, MemoryStream, bool>> Decryptors = new Dictionary<DMW_DATAENC, Func<byte[], string, MemoryStream, bool>>()
        {
            { DMW_DATAENC.Rijndael, RijndaelDecrypt }
        };

        static readonly byte[] DMW_SALT = new byte[] {
            0x5E, 0x2C, 0xE4, 0x06,
            0x79, 0x31, 0xFA, 0xA4,
            0x22, 0xCF, 0x31, 0x9A,
            0x5B, 0xB2, 0x89, 0x1F,
        };

        public override bool ImportModel(string path, ref Model model)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                string magicStr = fs.ReadString();
                if (magicStr != "DMW")
                {
                    MessageBox.Show("Error: Model is not of the DMW format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                int version = fs.ReadInt32();
                if (version < DMW_LOWEST_SUPPORTED_VERSION || version > DMW_CURRENT_VERSION)
                {
                    MessageBox.Show("Error: Model is of unsupported version " + version + "\n\nThis version of the DMW Suite only supports DMW models between versions " + DMW_LOWEST_SUPPORTED_VERSION + " and " + DMW_CURRENT_VERSION, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string readString;
                while ((readString = fs.ReadString()) != null)
                {
                    string[] splString = readString.Split(new char[] { ':' }, 2);
                    model.Metadata[splString[0]] = splString[1];
                }

                short encryptionTypeShrt = fs.ReadInt16();
                if (!Enum.IsDefined(typeof(DMW_DATAENC), encryptionTypeShrt))
                {
                    MessageBox.Show("Error: Model uses unsupported encryption method", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                DMW_DATAENC encryptionType = (DMW_DATAENC)encryptionTypeShrt;

                MemoryStream modelData = new MemoryStream();

                if (encryptionType != DMW_DATAENC.None)
                {
                    string encryptionKey = Prompt.Show("The model is encrypted. Enter the file's encryption key:");

                    if (encryptionKey.Trim().Length == 0)
                    {
                        MessageBox.Show("No encryption key entered. Model decryption cancelled.", "Encryption", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    MemoryStream encryptedData = new MemoryStream();
                    fs.CopyTo(encryptedData);

                    MemoryStream decryptedData = new MemoryStream();
                    if (!Decryptors[encryptionType](encryptedData.ToArray(), encryptionKey, decryptedData))
                    {
                        MessageBox.Show("Invalid encrypt key. Model decryption cancelled.", "Encryption", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    magicStr = decryptedData.ReadString();
                    if (magicStr != "DMW")
                    {
                        MessageBox.Show("Error: Decrypted data is corrupted. Model cannot be loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    int decVersion = decryptedData.ReadInt32();
                    if (decVersion != version)
                    {
                        MessageBox.Show("Error: Decrypted data has been modified or is corrupted. Model cannot be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    while ((readString = decryptedData.ReadString()) != null)
                    {
                        string[] splString = readString.Split(new char[] { ':' }, 2);

                        if(!model.Metadata.ContainsKey(splString[0])) //Hidden metadata
                        {
                            model.Metadata[splString[0]] = splString[1];
                        }
                        else if (model.Metadata[splString[0]] != splString[1])
                        {
                            MessageBox.Show("Error: Decrypted data has been modified or is corrupted. Model cannot be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }

                    decryptedData.CopyTo(modelData);
                }
                else
                {
                    fs.CopyTo(modelData);
                }

                model.Metadata["ComputedHash"] = Hash.Calculate(modelData.ToArray());

                modelData.Position = 0;
                while (modelData.Position < modelData.Length)
                {
                    byte mode = (byte)modelData.ReadByte();
                    int dataLen = modelData.ReadInt32();
                    long end = modelData.Position + dataLen;

                    while (modelData.Position < end)
                    {
                        switch (mode)
                        {
                            default: //Unsupported type. Just burn though it
                                modelData.ReadByte();
                                break;

                            case MARKER_VERT:
                                model.Vertices.Add(new Vertex(modelData.ReadFloat(), modelData.ReadFloat(), modelData.ReadFloat()));
                                break;

                            case MARKER_INDEX:
                                model.Faces.Add(new Face(modelData.ReadInt32(), modelData.ReadInt32(), modelData.ReadInt32()));
                                break;
                        }
                    }
                }
            }

            return true;
        }

        public override bool SaveModel(ref string path, ref Model model)
        {
            bool encrypted = false;
            string encryptionKey = "";

            if (MessageBox.Show("Encrypt content?", "Encrypt", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                encrypted = true;
                encryptionKey = Prompt.Show("Enter an encryption key:");

                if (encryptionKey.Trim().Length == 0)
                {
                    MessageBox.Show("No encryption key entered. Encryption cancelled.", "Encryption", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (!path.EndsWith(".enc.dmw"))
                    path = path.Substring(0, path.LastIndexOf('.')) + ".enc" + path.Substring(path.LastIndexOf('.'));
            }
            else
            {
                if (path.EndsWith(".enc.dmw"))
                    path = path.Substring(0, path.LastIndexOf(".enc")) + path.Substring(path.LastIndexOf('.'));
            }

            MemoryStream data = new MemoryStream(), vertStream = new MemoryStream(), indexStream = new MemoryStream();

            foreach (Vertex vert in model.Vertices)
            {
                vertStream.WriteFloat(vert.Position.X);
                vertStream.WriteFloat(vert.Position.Y);
                vertStream.WriteFloat(vert.Position.Z);
            }

            for (int i = 0; i < model.Faces.Count; ++i)
            {
                indexStream.WriteInt32(model.Faces[i].Vertex1);
                indexStream.WriteInt32(model.Faces[i].Vertex2);
                indexStream.WriteInt32(model.Faces[i].Vertex3);
            }

            data.WriteByte(MARKER_VERT);
            data.WriteInt32((int)vertStream.Length);
            vertStream.Position = 0; vertStream.CopyTo(data);

            data.WriteByte(MARKER_INDEX);
            data.WriteInt32((int)indexStream.Length);
            indexStream.Position = 0; indexStream.CopyTo(data);

            model.Metadata["DataHash"] = Hash.Calculate(data.ToArray());

            MemoryStream metadata = new MemoryStream();
            metadata.WriteString("DMW");
            metadata.WriteInt32(DMW_CURRENT_VERSION);

            foreach (KeyValuePair<StringKey, string> kvp in model.Metadata)
            {
                if (kvp.Key.Save)
                    metadata.WriteString(kvp.Key.KeyText + ":" + kvp.Value);
            }

            metadata.WriteInt16(0); //End of metadata notifier

            MemoryStream finalFile = new MemoryStream();
            metadata.Position = 0; metadata.CopyTo(finalFile);

            if (encrypted)
            {
                finalFile.WriteInt16((short)DefaultEncoder);
                MemoryStream toEncrypt = new MemoryStream();
                metadata.Position = 0; metadata.CopyTo(toEncrypt);
                data.Position = 0; data.CopyTo(toEncrypt);
                Encryptors[DefaultEncoder](toEncrypt.ToArray(), encryptionKey, finalFile);
            }
            else
            {
                finalFile.WriteInt16((short)DMW_DATAENC.None);
                data.Position = 0; data.CopyTo(finalFile);
            }

            File.WriteAllBytes(path, finalFile.ToArray());

            return true;
        }

        private static void RijndaelEncrypt(byte[] array, string key, MemoryStream toWriteTo)
        {
            Rijndael algorithm = Rijndael.Create();
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(key, DMW_SALT);

            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Key = rdb.GetBytes(32);
            algorithm.IV = rdb.GetBytes(16);

            ICryptoTransform ict = algorithm.CreateEncryptor();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Write))
                {
                    cs.WriteString("Rijndael");
                    cs.Write(array, 0, array.Length);
                    cs.FlushFinalBlock();
                    ms.Position = 0;
                    ms.CopyTo(toWriteTo);
                    cs.Close();
                }
            }
        }

        private static bool RijndaelDecrypt(byte[] array, string key, MemoryStream toWriteTo)
        {
            Rijndael algorithm = Rijndael.Create();
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(key, DMW_SALT);

            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Key = rdb.GetBytes(32);
            algorithm.IV = rdb.GetBytes(16);

            ICryptoTransform ict = algorithm.CreateDecryptor();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Write))
                    {
                        cs.Write(array, 0, array.Length);
                        cs.FlushFinalBlock();
                        ms.Position = 0;

                        string confirmStr = ms.ReadString();
                        if (confirmStr != "Rijndael")
                            return false;

                        ms.CopyTo(toWriteTo);
                        toWriteTo.Position = 0;
                        cs.Close();
                    }
                }
            }
            catch (CryptographicException)
            {
                return false;
            }

            return true;

        }
    }
}
