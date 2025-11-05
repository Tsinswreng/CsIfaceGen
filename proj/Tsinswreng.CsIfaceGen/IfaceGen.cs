namespace Tsinswreng.CsIfaceGen;


/// <summary>
/// 名須潙IfaceAttr、勿作IfaceAttrAttribute
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class IfaceGen
	:System.Attribute
	//,IIfaceGen
{
	/// <summary>
	/// 生成ʹ文件ʹ前綴
	/// </summary>
	public str? Name{get;set;}
	/// <summary>
	/// 父類型 接口或類 //TODO 改成 支持設多個
	/// </summary>
	public Type ParentType{get;set;}
	//ph: placeholder
	public str? PhFullType{get;set;}
	/// <summary>
	/// 去掉global::, 「_」替換成「__」,「.」,尖括號 替換成「_」
	/// 不支持泛型與nullable
	/// </summary>
	public str? PhIdentifierSafeFullType{get;set;}
	public str Template{get;set;} = "";
	/// <summary>
	/// 相對于項目根目錄之輸出目錄
	/// 若指定則寫文件至相應目錄ⁿ不用AddSource
	/// 開頭不用加/
	/// 若指定˪此、且項目中同時配置˪「把源生成器ʹʃ成ʹ輸出目錄(CompilerGeneratedFilesOutputPath)」、
	/// 則宜先清源生成器ʹʃ成ʹ輸出目錄、否則蜮衝突
	/// </summary>
	public str? OutDir{get;set;}

	/// <summary>
	/// 有值旹 輸出到單文件
	/// </summary>
	public str? OutFile{get;set;}
	/// <summary>
	/// 輸出到單文件時 額外賦于文件首部之內容
	/// </summary>
	public str? Head{get;set;}
	/// <summary>
	/// 輸出到單文件時 額外賦于文件尾部之內容
	/// </summary>
	public str? Tail{get;set;}
}
