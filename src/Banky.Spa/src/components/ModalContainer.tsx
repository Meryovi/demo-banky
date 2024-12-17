import { type PropsWithChildren, useEffect, useState } from "react";
import { createPortal } from "react-dom";

export default function ModalContainer(props: PropsWithChildren) {
  const [isOpen, setIsOpen] = useState(false);

  useEffect(() => setIsOpen(true), []); // This will trigger the 'open' transition...

  // I use a Portal here so that the content of the modal is rendered at the end of the body element...
  return createPortal(
    <dialog open className={`transition-all duration-1000 opacity-${isOpen ? 100 : 0}`}>
      <div className="fixed inset-0 bg-gray-900/80 z-40"></div>
      <div className="fixed inset-0 flex items-center justify-center z-50">
        <div className="bg-white p-6 rounded-lg shadow-lg max-w-sm w-full">{props.children}</div>
      </div>
    </dialog>,
    document.body,
  );
}
