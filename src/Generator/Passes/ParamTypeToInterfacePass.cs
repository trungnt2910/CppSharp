﻿using CppSharp.AST;

namespace CppSharp.Passes
{
    public class ParamTypeToInterfacePass : TranslationUnitPass
    {
        public override bool VisitFunctionDecl(Function function)
        {
            if (!function.IsOperator)
            {
                ChangeToInterfaceType(function.ReturnType);
                foreach (Parameter parameter in function.Parameters)
                {
                    ChangeToInterfaceType(parameter.QualifiedType);
                }
            }
            return base.VisitFunctionDecl(function);
        }

        private static void ChangeToInterfaceType(QualifiedType type)
        {
            var tagType = type.Type as TagType;
            if (tagType == null)
            {
                var pointerType = type.Type as PointerType;
                if (pointerType != null)
                    tagType = pointerType.Pointee as TagType;
            }
            if (tagType != null)
            {
                var @class = tagType.Declaration as Class;
                if (@class != null)
                {
                    var @interface = @class.Namespace.Classes.Find(c => c.OriginalClass == @class);
                    if (@interface != null)
                        tagType.Declaration = @interface;
                }
            }
        }
    }
}
