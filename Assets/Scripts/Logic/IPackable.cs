using System.Collections;
using UnityEngine;

namespace SangjiagouCore
{
    /// <summary>
    /// 可以打包以适应JSON序列化的类
    /// </summary>
    /// <typeparam name="Package">所打包的类型。因为要求能够适应JSON序列化，要求其成员必须或是基本类型，或是基本类型为元素的容器，或是包类型</typeparam>
    public interface IPackable<Package>
    {
        /// <summary>
        /// 从对象提取数据打包，如果有依赖该对象创建的其他实现IPackable的对象的成员，那么进行递归打包。
        /// </summary>
        /// <returns>所打的包</returns>
        Package Pack();
        /// <summary>
        /// 从包中提取数据给对象中的基本类型成员，并将包的引用暂存在对象中，以便后续进行Relink。<br/>
        /// 注意：如果对象中有对其他对象的成员引用，请务必在所有对象的Unpack方法调用完毕后，再调用一轮Relink方法
        /// </summary>
        /// <param name="pkg">所要解析的包</param>
        void Unpack(Package pkg);
        /// <summary>
        /// 从暂存于对象的包中提取数据，并依此递归地处理对象中的引用成员，最后解除包的引用。若包的引用已解除，则Relink不执行任何操作，直接返回。<br/>
        /// 由于各对象中可能有对其他对象的引用，而调用Unpack是该对象可能还未创建，所以需要在Unpack后进行Relink
        /// </summary>
        void Relink();
    }
}