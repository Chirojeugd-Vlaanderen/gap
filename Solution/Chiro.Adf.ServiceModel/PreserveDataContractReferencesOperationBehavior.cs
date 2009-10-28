using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace Chiro.Adf.ServiceModel
{
    /// <summary>
    /// TODO: Documenteren!
    /// </summary>
	public class ReferencePreservingDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
	{
        /// <summary>
        /// TODO: Documenteren
        /// </summary>
        /// <param name="operationDescription"></param>
		public ReferencePreservingDataContractSerializerOperationBehavior(OperationDescription operationDescription) : base(operationDescription) { }

        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ns"></param>
        /// <param name="knownTypes"></param>
        /// <returns></returns>
		public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
		{
			return CreateDataContractSerializer(type, name, ns, knownTypes);
		}

        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ns"></param>
        /// <param name="knownTypes"></param>
        /// <returns></returns>
        private static XmlObjectSerializer CreateDataContractSerializer(Type type, string name, string ns, IList<Type> knownTypes)
		{
			return CreateDataContractSerializer(type, name, ns, knownTypes);
        }
        
        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="ns"></param>
        /// <param name="knownTypes"></param>
        /// <returns></returns>
		public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
		{
			return new DataContractSerializer(type, name, ns, knownTypes,
                0x7FFF /*maxItemsInObjectGraph*/,
                false/*ignoreExtensionDataObject*/,
                true/*preserveObjectReferences*/,
                null/*dataContractSurrogate*/);
		}
	}

    /// <summary>
    /// TODO: Documenteren!
    /// </summary>
	public class ReferencePreservingDataContractFormatAttribute : Attribute, IOperationBehavior
	{
        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
        /// <param name="description"></param>
        /// <param name="parameters"></param>
		public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters) { }
        
        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
        /// <param name="description"></param>
        /// <param name="proxy"></param>
		public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
		{
            IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior(description);
            innerBehavior.ApplyClientBehavior(description, proxy);
		}

        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
        /// <param name="description"></param>
        /// <param name="dispatch"></param>
        public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
		{
            IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior(description);
            innerBehavior.ApplyDispatchBehavior(description, dispatch);
		}
        
        /// <summary>
        /// TODO: Documenteren!
        /// </summary>
        /// <param name="description"></param>
		public void Validate(OperationDescription description)
		{

		}
	}
}
