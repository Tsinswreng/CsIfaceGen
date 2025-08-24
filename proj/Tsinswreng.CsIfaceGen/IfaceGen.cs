namespace Tsinswreng.CsIfaceGen;


/// <summary>
/// 名須潙IfaceAttr、勿作IfaceAttrAttribute
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class IfaceGen
	:System.Attribute
	//,IIfaceGen
{

	public str? Name{get;set;}
	/// <summary>
	/// 父類型 接口或類
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
}
