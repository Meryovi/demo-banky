const PrimaryButtonClass =
  "bg-indigo-700 text-white py-2 px-4 rounded hover:bg-indigo-950 hover:text-gray-300 hover:cursor-pointer transition-colors duration-400";

export function PrimaryButton(props: React.ButtonHTMLAttributes<HTMLButtonElement>) {
  const { children, className, ...rest } = props;
  const mergedClasses = className ? `${PrimaryButtonClass} ${className}` : PrimaryButtonClass;

  return (
    <button type="button" className={mergedClasses} {...rest}>
      {children}
    </button>
  );
}

const LinkButtonClass = "text-blue-700 py-1 hover:text-indigo-500 hover:cursor-pointer transition-colors duration-400";

export function LinkButton(props: React.ButtonHTMLAttributes<HTMLButtonElement>) {
  const { children, className, ...rest } = props;
  const mergedClasses = className ? `${LinkButtonClass} ${className}` : LinkButtonClass;

  return (
    <button type="button" className={mergedClasses} {...rest}>
      {children}
    </button>
  );
}
