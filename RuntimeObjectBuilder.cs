using Planetbase;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;
using PlanetbaseFramework;
using UnityEngine;
using XMLModloaderModV2.Deserializers.Version2;

namespace XMLModloaderModV2
{
    public class RuntimeObjectBuilder
    {
        public static string AssemblyNameString => "DynamicMods";
        public static string Version => "1.0.0.0";
        public static AssemblyName AssemblyName { get; }
        public static AssemblyBuilder AssemblyBuilder { get; }
        public static ModuleBuilder ModuleBuilder { get; }

        public static string XmlModNamespace => "XMLMods";

        public string ModNamespace { get; }

        static RuntimeObjectBuilder()
        {
            AssemblyName = new AssemblyName(AssemblyNameString + ",Version=" + Version);
            AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(AssemblyNameString, AssemblyNameString + ".dll");
        }

        public RuntimeObjectBuilder(string modNamespace)
        {
            ModNamespace = modNamespace;
        }

        public static Type CreateXmlModType(string modName, string internalName, Type parentType)
        {
            TypeBuilder modClassBuilder = ModuleBuilder.DefineType(XmlModNamespace + "." + internalName + "." + internalName,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.BeforeFieldInit,
                parentType);

            modClassBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            CustomAttributeBuilder xmlRootAttributeBuilder = new CustomAttributeBuilder(typeof(XmlRootAttribute).GetConstructor(new Type[] { typeof(string) }), new object[] { "Mod" });

            modClassBuilder.SetCustomAttribute(xmlRootAttributeBuilder);

            CustomAttributeBuilder serializableAttributeBuilder = new CustomAttributeBuilder(typeof(SerializableAttribute).GetConstructor(Type.EmptyTypes), new object[0]);

            modClassBuilder.SetCustomAttribute(serializableAttributeBuilder);

            PropertyBuilder modNameBuilder =
                modClassBuilder.DefineProperty("ModName", PropertyAttributes.HasDefault, typeof(string), null);

            MethodBuilder modNameGetBuilder = modClassBuilder.DefineMethod("get_ModName",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual |
                MethodAttributes.SpecialName, typeof(string), Type.EmptyTypes);

            ILGenerator modNameGetGenerator = modNameGetBuilder.GetILGenerator();

            modNameGetGenerator.Emit(OpCodes.Ldstr, modName);
            modNameGetGenerator.Emit(OpCodes.Ret);

            modNameBuilder.SetGetMethod(modNameGetBuilder);

            Type modBaseType = modClassBuilder.CreateType();

            AssemblyBuilder.Save(AssemblyNameString + ".dll");

            return modBaseType;
        }

        public Tech CreateTechObject(string className)
        {
            return CreateGenericObject<Tech>(className);
        }

        public BuildingModuleType CreateBuildingModuleTypeObject(string className)
        {
            return CreateGenericObject<BuildingModuleType>(className);
            //XMLModuleType newModuleType = CreateGenericObject<XMLModuleType>(className);

            //TypeList<ModuleType, ModuleTypeList>.getInstance().add(newModuleType);

            //return newModuleType;
        }

        private T CreateGenericObject<T>(string className)
        {
            TypeBuilder classBuilder = ModuleBuilder.DefineType(XmlModNamespace + "." + ModNamespace + "." + className,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                typeof(T));

            classBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            Type TType = classBuilder.CreateType();

            T newTech = (T)Activator.CreateInstance(TType);

            return newTech;
        }
    }
}
