#define XLSX

using Excel;
using ShipDock.Applications;
using ShipDock.Tools;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

namespace ShipDock.Editors
{
    public class ExcelTool
    {
        /// <summary>
        /// 配置转换的相关设置
        /// </summary>
        public class ExeclSetting
        {
            /// <summary>配置数据的起始行列定义</summary>
            public ExeclDefination dataStart;
            /// <summary>类信息（类名等）的行列定义</summary>
            public ExeclDefination classDefine;
            /// <summary>类模板信息（映射对象、静态类等）的行列定义</summary>
            public ExeclDefination classType;
            /// <summary>id字段名的行列定义</summary>
            public ExeclDefination IDFieldName;
            /// <summary>字段类型的行列定义</summary>
            public ExeclDefination dataType;
            /// <summary>字段名的行列定义</summary>
            public ExeclDefination keyFieldDef;
            /// <summary>注释的行列定义</summary>
            public ExeclDefination noteFieldDef;
        }

        public struct ExeclDefination
        {
            public int row;
            public int column;
        }

        public static ExeclSetting Setting { get; set; }

        private static void InitExeclDefs()
        {
            Setting = new ExeclSetting
            {
                classDefine = new ExeclDefination
                {
                    row = 0,
                    column = 0,
                },
                classType = new ExeclDefination
                {
                    row = 0,
                    column = 1,
                },
                IDFieldName = new ExeclDefination
                {
                    row = 0,
                    column = 2,
                },
                dataStart = new ExeclDefination
                {
                    row = 5,
                    column = 1,
                },
                dataType = new ExeclDefination
                {
                    row = 1,
                    column = 0,
                },
                keyFieldDef = new ExeclDefination
                {
                    row = 3,
                    column = 0,
                },
                noteFieldDef = new ExeclDefination
                {
                    row = 4,
                    column = 0,
                }
            };
        }

        public class ClassTemplateInfo
        {
            public StringBuilder sb;
            public DataRowCollection collect;
            public int notesRow;
            public int dataStartCol;
            public string className;
            public string typeValue;
            public string IDName;
            /// <summary>类名标记</summary>
            public string rplClsName = "%cls%";
            /// <summary>id字段名标记</summary>
            public string idKeyName = "%id%";
            /// <summary>字段类型标记</summary>
            public string typeName = "%type%";
            /// <summary>字段名标记</summary>
            public string fieldName = "%fieldName%";
            /// <summary>解析配置的代码段落标记</summary>
            public string parseCode = "%parseCode%";
            /// <summary>其他代码段落标记（类的字段等代码）</summary>
            public string codes = "%codes%";
            /// <summary注释标记</summary>
            public string notes = "%notes%";
        }

        /// <summary>
        /// 读取表数据，生成对应的数组
        /// </summary>
        /// <param name="filePath">excel文件全路径</param>
        /// <returns>Item数组</returns>
        public static void CreateItemArrayWithExcel(string filePath, out string relativeName)
        {
            InitExeclDefs();

            DataRowCollection collect = ReadExcel(filePath, out int rowSize, out int colSize);//获得表数据
            int col = Setting.classDefine.column, row = Setting.classDefine.row;
            string className = collect[row][col].ToString();//类名

            col = Setting.classType.column;
            row = Setting.classType.row;
            string classType = collect[row][col].ToString();//类模板的类别

            col = Setting.IDFieldName.column;
            row = Setting.IDFieldName.row;
            string IDName = collect[row][col].ToString();//id字段的名称

            int dataStartRow = Setting.dataStart.row;
            int dataStartCol = Setting.dataStart.column;

            StringBuilder sb = new StringBuilder();

            int typeRow = Setting.dataType.row;//类型名所在行
            int keyRow = Setting.keyFieldDef.row;//字段名所在行
            int notesRow = Setting.noteFieldDef.row;//注释所在行
            object IDTypeCell = collect[typeRow][dataStartCol];
            string typeValue = GetTypeValueByCell(IDTypeCell.ToString());

            ClassTemplateInfo info = new ClassTemplateInfo
            {
                sb = sb,
                collect = collect,
                className = className,
                notesRow = notesRow,
                dataStartCol = dataStartCol,
                typeValue = typeValue,
                IDName = IDName,
            };

            switch (classType)
            {
                case "mapper":
                    TemplateMapperConfig(info);
                    break;
                case "const"://常量类
                    sb.Append("namespace StaticConfig");
                    sb.Append("{");
                    sb.Append("    public static class %cls%".Replace(info.rplClsName, className));
                    sb.Append("    {");
                    sb.Append("    }");
                    sb.Append("}");
                    break;
            }
            string field;
            string script = sb.ToString();
            sb.Clear();

            object item;
            string parserCode = IDName.Append(GetParseMethodByType(IDTypeCell.ToString()));
            for (int i = dataStartCol + 1; i < colSize; i++)//从id列顺延下一个字段开始构建剩余的脚本
            {
                item = collect[typeRow][i];//类型
                typeValue = item.ToString();
                if (string.IsNullOrEmpty(typeValue))
                {
                    colSize = i;//读取到无效的表头
                    break;
                }
                typeValue = GetTypeValueByCell(typeValue);

                item = collect[keyRow][i];//字段名
                field = item.ToString();
                sb.Append("/// <summary>");
                sb.Append("\r\n        /// %notes%").Replace(info.notes, collect[notesRow][i].ToString());
                sb.Append("\r\n        /// <summary>\r\n        ");
                sb.Append("public %type% %fieldName%;\r\n").Replace(info.typeName, typeValue).Replace(info.fieldName, field);
                sb.Append("        ");

                parserCode = parserCode.Append(field, GetParseMethodByType(collect[typeRow][i].ToString()));
            }
            script = script.Replace(info.codes, sb.ToString());
            script = script.Replace(info.parseCode, parserCode);
            sb.Clear();

            string path = Application.dataPath.Append("/HotFix~/StaticConfigs/".Append(className, ".cs"));
            FileOperater.WriteUTF8Text(script, path);//生成代码

            ByteBuffer byteBuffer = ByteBuffer.Allocate(0);
            int dataRealSize = rowSize - dataStartRow;
            byteBuffer.WriteInt(dataRealSize);
            for (int i = dataStartRow; i < rowSize; i++)
            {
                Debug.Log("Writing in ID = " + collect[i][dataStartCol].ToString());
                for (int j = dataStartCol; j < colSize; j++)//从id列顺延下一个字段开始构建剩余的脚本
                {
                    item = collect[typeRow][j];//类型
                    typeValue = item.ToString();

                    field = collect[i][j].ToString();//数据

                    WriteField(ref byteBuffer, typeValue, field);
                }
            }
            Debug.Log("Write finished.. length = " + byteBuffer.GetCapacity());

            relativeName = "configs/".Append(className.ToLower(), ".bytes");
            path = AppPaths.DataPathResDataRoot.Append(relativeName);
            FileOperater.WriteBytes(byteBuffer.ToArray(), path);//生成配置数据
        }

        /// <summary>
        /// 用于映射到字典的可实例化对象的配置
        /// </summary>
        /// <param name="info"></param>
        private static void TemplateMapperConfig(ClassTemplateInfo info)
        {
            StringBuilder sb = info.sb;
            sb.Append("using ShipDock.Config;\r\n\r\n");
            sb.Append("using ShipDock.Tools;\r\n\r\n");
            sb.Append("namespace StaticConfig\r\n");
            sb.Append("{\r\n");
            sb.Append("    public class %cls% : IConfig\r\n".Replace(info.rplClsName, info.className));
            sb.Append("    {\r\n");
            sb.Append("        /// <summary>\r\n");
            sb.Append("        /// %notes%\r\n").Replace(info.notes, info.collect[info.notesRow][info.dataStartCol].ToString());
            sb.Append("        /// <summary>\r\n");
            sb.Append("        public %type% %id%;\r\n\r\n").Replace(info.typeName, info.typeValue).Replace(info.idKeyName, info.IDName);//ID
            sb.Append("        %codes%\r\n");//此处容纳其他代码
            sb.Append("        public string CRCValue { get; }\r\n\r\n");
            sb.Append("        public %type% GetID()\r\n").Replace(info.typeName, info.typeValue);
            sb.Append("        {\r\n");
            sb.Append("            return id;\r\n");
            sb.Append("        }\r\n\r\n");
            sb.Append("        public void Parse(ByteBuffer buffer)\r\n");
            sb.Append("        {\r\n");
            sb.Append("            %parseCode%\r\n");
            sb.Append("        }\r\n\r\n");
            sb.Append("    }\r\n");
            sb.Append("}\r\n");
        }

        private static void WriteField(ref ByteBuffer byteBuffer, string typeValue, string field)
        {
            switch(typeValue)
            {
                case "int32":
                    Debug.Log("Write in int32, " + int.Parse(field));
                    byteBuffer.WriteInt(int.Parse(field));
                    break;
                case "string":
                    Debug.Log("Write in string, " + field);
                    byteBuffer.WriteString(field);
                    break;
                case "float":
                    Debug.Log("Write in float, " + float.Parse(field));
                    byteBuffer.WriteFloat(float.Parse(field));
                    break;
                case "bool":
                    Debug.Log("Write in bool, " + field);
                    int value = field == "TRUE" ? 1 : 0;
                    byteBuffer.WriteInt(value);
                    break;
            }
        }

        private static string GetTypeValueByCell(string v)
        {
            string result;
            switch (v)
            {
                case "int32":
                    result = "int";
                    break;
                case "string":
                    result = "string";
                    break;
                case "float":
                    result = "float";
                    break;
                case "bool":
                    result = "bool";
                    break;
                default:
                    result = "object";
                    break;
            }
            return result;
        }

        private static string GetParseMethodByType(string v)
        {
            string result = string.Empty;
            switch (v)
            {
                case "int32":
                    result = " = buffer.ReadInt();\r\n";
                    break;
                case "string":
                    result = " = buffer.ReadString();\r\n";
                    break;
                case "float":
                    result = " = buffer.ReadFloat();\r\n";
                    break;
                case "bool":
                    result = " = buffer.ReadInt() != 0;\r\n";
                    break;
            }
            return result.Append("            ");
        }

        /// <summary>
        /// 读取excel文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="rowSize">行数</param>
        /// <param name="colSize">列数</param>
        /// <returns></returns>
        static DataRowCollection ReadExcel(string filePath, out int rowSize, out int colSize, int sheetIndex = 0)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
#if XLSX
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);//xlsx
#else
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);//xls
#endif

            DataSet result = excelReader.AsDataSet();
            DataTable dataTable = result.Tables[sheetIndex];//下标0表示excel文件中第一张表的数据
            rowSize = dataTable.Rows.Count;
            colSize = dataTable.Columns.Count;
            return dataTable.Rows;
        }
    }
}