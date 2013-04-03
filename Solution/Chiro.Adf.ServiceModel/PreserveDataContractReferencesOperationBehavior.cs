using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace Chiro.Adf.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class ReferencePreservingDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
	{

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencePreservingDataContractSerializerOperationBehavior"/> class.
        /// </summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <remarks></remarks>
		public ReferencePreservingDataContractSerializerOperationBehavior(OperationDescription operationDescription) : base(operationDescription) { }


        /// <summary>
        /// Creates an instance of a class that inherits from <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> for serialization and deserialization processes.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type"/> to create the serializer for.</param>
        /// <param name="name">The name of the generated type.</param>
        /// <param name="ns">The namespace of the generated type.</param>
        /// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IList`1"/> of <see cref="T:System.Type"/> that contains known types.</param>
        /// <returns>An instance of a class that inherits from the <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> class.</returns>
        /// <remarks></remarks>
		public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
		{
			return CreateDataContractSerializer(type, name, ns, knownTypes);
		}


        /// <summary>
        /// Creates the data contract serializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="ns">The ns.</param>
        /// <param name="knownTypes">The known types.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static XmlObjectSerializer CreateDataContractSerializer(Type type, string name, string ns, IList<Type> knownTypes)
		{
			return CreateDataContractSerializer(type, name, ns, knownTypes);
        }


        /// <summary>
        /// Creates an instance of a class that inherits from <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> for serialization and deserialization processes with an <see cref="T:System.Xml.XmlDictionaryString"/> that contains the namespace.
        /// </summary>
        /// <param name="type">The type to serialize or deserialize.</param>
        /// <param name="name">The name of the serialized type.</param>
        /// <param name="ns">An <see cref="T:System.Xml.XmlDictionaryString"/> that contains the namespace of the serialized type.</param>
        /// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IList`1"/> of <see cref="T:System.Type"/> that contains known types.</param>
        /// <returns>An instance of a class that inherits from the <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> class.</returns>
        /// <remarks></remarks>
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
    /// 
    /// </summary>
    /// <remarks></remarks>
	public class ReferencePreservingDataContractFormatAttribute : Attribute, IOperationBehavior
	{

        /// <summary>
        /// Adds the binding parameters.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks></remarks>
		public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters) { }


        /// <summary>
        /// Applies the client behavior.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="proxy">The proxy.</param>
        /// <remarks></remarks>
		public void ApplyClientBehavior(OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy)
		{
            IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior(description);
            innerBehavior.ApplyClientBehavior(description, proxy);
		}


        /// <summary>
        /// Applies the dispatch behavior.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="dispatch">The dispatch.</param>
        /// <remarks></remarks>
        public void ApplyDispatchBehavior(OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch)
		{
            IOperationBehavior innerBehavior = new ReferencePreservingDataContractSerializerOperationBehavior(description);
            innerBehavior.ApplyDispatchBehavior(description, dispatch);
		}


        /// <summary>
        /// Validates the specified description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <remarks></remarks>
		public void Validate(OperationDescription description)
		{

		}
	}
}
